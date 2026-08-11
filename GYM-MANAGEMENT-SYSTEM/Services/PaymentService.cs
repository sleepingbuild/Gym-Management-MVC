using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using GYM_MANAGEMENT_SYSTEM.VNPay;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMembershipPackageRepository _packageRepository;
        private readonly IMembershipRenewalService _renewalService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly VNPayConfig _vnpayConfig;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMembershipRepository membershipRepository,
            IMembershipPackageRepository packageRepository,
            IMembershipRenewalService renewalService,
            UserManager<ApplicationUser> userManager,
            IOptions<VNPayConfig> vnpayConfig)
        {
            _paymentRepository = paymentRepository;
            _membershipRepository = membershipRepository;
            _packageRepository = packageRepository;
            _renewalService = renewalService;
            _userManager = userManager;
            _vnpayConfig = vnpayConfig.Value;
        }

        public async Task<Payment> CreatePaymentAsync(PaymentCreateViewModel model)
        {
            var membership = await _membershipRepository.GetByIdAsync(model.MembershipId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            var payment = new Payment
            {
                UserId = model.UserId,
                MembershipId = model.MembershipId,
                Amount = model.Amount,
                Method = model.Method,
                Status = "Pending",
                TransactionId = "",
                PaymentInfo = model.PaymentInfo,
                CreatedAt = DateTime.UtcNow
            };

            return await _paymentRepository.AddAsync(payment);
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            return await _paymentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId)
        {
            return await _paymentRepository.GetByUserIdAsync(userId);
        }

        public async Task<Payment> UpdatePaymentStatusAsync(int id, string status, string transactionId = "")
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thanh toán.");
            }

            payment.Status = status;
            if (!string.IsNullOrEmpty(transactionId))
            {
                payment.TransactionId = transactionId;
            }

            return await _paymentRepository.UpdateAsync(payment);
        }

        public async Task<Payment> ConfirmLocalPaymentAsync(PaymentCreateViewModel model)
        {
            var membership = await _membershipRepository.GetByIdAsync(model.MembershipId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            var payment = new Payment
            {
                UserId = model.UserId,
                MembershipId = model.MembershipId,
                Amount = model.Amount,
                Method = string.IsNullOrWhiteSpace(model.Method) ? "Tại quầy" : model.Method,
                Status = "Success",
                TransactionId = $"LOCAL-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                PaymentInfo = model.PaymentInfo,
                CreatedAt = DateTime.UtcNow
            };
            payment = await _paymentRepository.AddAsync(payment);

            if (membership.Status == "Pending")
            {
                var package = membership.MembershipPackage
                    ?? await _packageRepository.GetByIdAsync(membership.MembershipPackageId);
                var durationDays = package?.DurationDays ?? 0;

                membership.StartDate = DateTime.UtcNow;
                membership.EndDate = DateTime.UtcNow.AddDays(durationDays);
                membership.Status = "Active";
                await _membershipRepository.UpdateAsync(membership);
            }
            else
            {
                await _renewalService.RenewMembershipAsync(membership.Id);
            }

            return payment;
        }

        public async Task<Membership?> GetCurrentMembershipAsync(string userId)
        {
            var latestSuccess = (await _paymentRepository.GetByUserIdAsync(userId))
                .Where(p => p.Status == "Success" && p.MembershipId.HasValue)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            if (latestSuccess == null)
            {
                return null;
            }

            return await _membershipRepository.GetByIdAsync(latestSuccess.MembershipId!.Value);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByMembershipAsync(int membershipId)
        {
            return await _paymentRepository.GetByMembershipIdAsync(membershipId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentHistoryAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var payments = await _paymentRepository.GetByUserIdAsync(userId);

            if (fromDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt <= toDate.Value);
            }

            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<IEnumerable<Payment>> GetPaymentHistoryByStatusAsync(string userId, string status)
        {
            var payments = await _paymentRepository.GetByUserIdAsync(userId);
            return payments.Where(p => p.Status == status)
                           .OrderByDescending(p => p.CreatedAt);
        }

        public async Task<IEnumerable<Payment>> SearchPaymentsAsync(string userId, string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var payments = await _paymentRepository.GetByUserIdAsync(userId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                payments = payments.Where(p =>
                    p.TransactionId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.PaymentInfo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Method.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (fromDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt <= toDate.Value);
            }

            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<PaymentStatisticsViewModel> GetPaymentStatisticsAsync(string userId)
        {
            var payments = await _paymentRepository.GetByUserIdAsync(userId);

            var stats = new PaymentStatisticsViewModel
            {
                TotalPayments = payments.Count(),
                TotalAmount = payments.Where(p => p.Status == "Success").Sum(p => p.Amount),
                SuccessCount = payments.Count(p => p.Status == "Success"),
                PendingCount = payments.Count(p => p.Status == "Pending"),
                FailedCount = payments.Count(p => p.Status == "Failed"),
                RecentPayments = payments
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new PaymentSummaryViewModel
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        Method = p.Method,
                        Status = p.Status,
                        CreatedAt = p.CreatedAt
                    }).ToList()
            };


            stats.TotalSuccessAmount = stats.TotalAmount;


            stats.SuccessRate = stats.TotalPayments > 0
                ? Math.Round((double)stats.SuccessCount / stats.TotalPayments * 100, 1)
                : 0;

            return stats;
        }


        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var payments = await _paymentRepository.GetAllAsync();

            if (fromDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt <= toDate.Value);
            }

            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsByStatusAsync(string status)
        {
            var payments = await _paymentRepository.GetAllAsync();
            return payments.Where(p => p.Status == status)
                           .OrderByDescending(p => p.CreatedAt);
        }

        public async Task<IEnumerable<Payment>> SearchAllPaymentsAsync(string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var payments = (await _paymentRepository.GetAllAsync()).ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var matchedByTransaction = payments.Where(p =>
                    p.TransactionId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.PaymentInfo.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Method.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).Select(p => p.Id).ToHashSet();

                var matchedUserIds = _userManager.Users
                    .Where(u =>
                        (u.FullName != null && u.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (u.Email != null && u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(u => u.Id)
                    .ToHashSet();

                payments = payments.Where(p =>
                    matchedByTransaction.Contains(p.Id) || matchedUserIds.Contains(p.UserId)
                ).ToList();
            }

            if (fromDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                payments = payments.Where(p => p.CreatedAt <= toDate.Value).ToList();
            }

            return payments.OrderByDescending(p => p.CreatedAt);
        }

        public async Task<PaymentStatisticsViewModel> GetOverallPaymentStatisticsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            var stats = new PaymentStatisticsViewModel
            {
                TotalPayments = payments.Count(),
                TotalAmount = payments.Where(p => p.Status == "Success").Sum(p => p.Amount),
                SuccessCount = payments.Count(p => p.Status == "Success"),
                PendingCount = payments.Count(p => p.Status == "Pending"),
                FailedCount = payments.Count(p => p.Status == "Failed"),
                RecentPayments = payments
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new PaymentSummaryViewModel
                    {
                        Id = p.Id,
                        Amount = p.Amount,
                        Method = p.Method,
                        Status = p.Status,
                        CreatedAt = p.CreatedAt
                    }).ToList()
            };

            stats.TotalSuccessAmount = stats.TotalAmount;

            stats.SuccessRate = stats.TotalPayments > 0
                ? Math.Round((double)stats.SuccessCount / stats.TotalPayments * 100, 1)
                : 0;

            return stats;
        }

        public async Task<Dictionary<string, (string FullName, string Email)>> GetMemberDisplayInfoAsync(IEnumerable<string> userIds)
        {
            var idSet = userIds.ToHashSet();

            var users = _userManager.Users
                .Where(u => idSet.Contains(u.Id))
                .ToList();

            return users.ToDictionary(
                u => u.Id,
                u => (FullName: string.IsNullOrWhiteSpace(u.FullName) ? (u.Email ?? u.Id) : u.FullName, Email: u.Email ?? string.Empty)
            );
        }
    }
}