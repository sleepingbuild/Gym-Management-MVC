using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using GYM_MANAGEMENT_SYSTEM.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IMembershipService _membershipService;
        private readonly VNPayConfig _vnpayConfig;

        public PaymentController(
            IPaymentService paymentService,
            IMembershipService membershipService,
            IOptions<VNPayConfig> vnpayConfig)
        {
            _paymentService = paymentService;
            _membershipService = membershipService;
            _vnpayConfig = vnpayConfig.Value;
        }

        // GET: /Payment — gộp về đúng 1 trang lịch sử thanh toán duy nhất
        public IActionResult Index()
        {
            return RedirectToAction(nameof(History));
        }

        // GET: /Payment/Create
        public async Task<IActionResult> Create(int membershipId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var membership = await _membershipService.GetByIdAsync(membershipId);
            if (membership == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy gói tập.";
                return RedirectToAction("Index", "Membership");
            }

            var isRenewal = membership.Status != "Pending";
            var packageName = membership.MembershipPackage?.Name ?? $"#{membershipId}";

            var viewModel = new PaymentCreateViewModel
            {
                UserId = userId,
                MembershipId = membershipId,
                Amount = membership.MembershipPackage?.Price ?? 0,
                Method = "Tại quầy",
                PaymentInfo = isRenewal
                    ? $"Gia hạn gói tập {packageName}"
                    : $"Thanh toán gói tập {packageName}"
            };

            return View(viewModel);
        }

        // POST: /Payment/Create — xác nhận thanh toán ngay, không còn chuyển
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _paymentService.ConfirmLocalPaymentAsync(model);
                TempData["SuccessMessage"] = "Thanh toán thành công! Gói tập của bạn đã được kích hoạt.";
                return RedirectToAction(nameof(History));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Payment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            var viewModel = new PaymentIndexViewModel
            {
                Id = payment.Id,
                UserId = payment.UserId,
                MembershipId = payment.MembershipId,
                Amount = payment.Amount,
                Method = payment.Method,
                Status = payment.Status,
                TransactionId = payment.TransactionId,
                PaymentInfo = payment.PaymentInfo,
                CreatedAt = payment.CreatedAt
            };

            return View(viewModel);
        }

        // GET: /Payment/History
        public async Task<IActionResult> History(PaymentHistoryFilterViewModel filter)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }


            IEnumerable<Payment> payments;

            if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "Tất cả")
            {
                payments = await _paymentService.GetPaymentHistoryByStatusAsync(userId, filter.Status);
            }
            else if (!string.IsNullOrEmpty(filter.SearchTerm) || filter.FromDate.HasValue || filter.ToDate.HasValue)
            {
                payments = await _paymentService.SearchPaymentsAsync(userId, filter.SearchTerm, filter.FromDate, filter.ToDate);
            }
            else
            {
                payments = await _paymentService.GetPaymentHistoryAsync(userId);
            }

            var viewModels = payments.Select(p => new PaymentIndexViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                MembershipId = p.MembershipId,
                Amount = p.Amount,
                Method = p.Method,
                Status = p.Status,
                TransactionId = p.TransactionId,
                PaymentInfo = p.PaymentInfo,
                CreatedAt = p.CreatedAt
            }).ToList();

            var stats = await _paymentService.GetPaymentStatisticsAsync(userId);
            ViewBag.Statistics = stats;
            var pendingMemberships = (await _membershipService.GetUserMembershipsAsync(userId))
                .Where(m => m.Status == "Pending")
                .ToList();
            ViewBag.PendingMemberships = pendingMemberships;

            return View(viewModels);
        }

        // GET: /Payment/AdminHistory
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminHistory(PaymentHistoryFilterViewModel filter)
        {
            IEnumerable<Payment> payments;

            if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "Tất cả")
            {
                payments = await _paymentService.GetAllPaymentsByStatusAsync(filter.Status);
            }
            else if (!string.IsNullOrEmpty(filter.SearchTerm) || filter.FromDate.HasValue || filter.ToDate.HasValue)
            {
                payments = await _paymentService.SearchAllPaymentsAsync(filter.SearchTerm, filter.FromDate, filter.ToDate);
            }
            else
            {
                payments = await _paymentService.GetAllPaymentsAsync();
            }

            var paymentList = payments.ToList();
            var memberInfo = await _paymentService.GetMemberDisplayInfoAsync(paymentList.Select(p => p.UserId).Distinct());

            var viewModels = paymentList.Select(p =>
            {
                memberInfo.TryGetValue(p.UserId, out var info);
                return new PaymentIndexViewModel
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    MembershipId = p.MembershipId,
                    Amount = p.Amount,
                    Method = p.Method,
                    Status = p.Status,
                    TransactionId = p.TransactionId,
                    PaymentInfo = p.PaymentInfo,
                    CreatedAt = p.CreatedAt,
                    MemberName = info.FullName ?? p.UserId,
                    MemberEmail = info.Email ?? string.Empty
                };
            }).ToList();

            var stats = await _paymentService.GetOverallPaymentStatisticsAsync();
            ViewBag.Statistics = stats;

            return View(viewModels);
        }

        // GET: /Payment/AdminExportCSV
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminExportCSV(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var payments = (await _paymentService.GetAllPaymentsAsync(fromDate, toDate)).ToList();
            var memberInfo = await _paymentService.GetMemberDisplayInfoAsync(payments.Select(p => p.UserId).Distinct());

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Mã GD,Thành viên,Email,Số tiền,Phương thức,Trạng thái,Mã GD,Ngày,Thông tin");

            foreach (var p in payments)
            {
                memberInfo.TryGetValue(p.UserId, out var info);
                csv.AppendLine($"{p.Id},{info.FullName ?? p.UserId},{info.Email},{p.Amount:N0},{p.Method},{p.Status},{p.TransactionId},{p.CreatedAt:dd/MM/yyyy HH:mm},{p.PaymentInfo}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"admin_payment_history_{DateTime.Now:yyyyMMdd}.csv");
        }

        // GET: /Payment/Statistics
        public async Task<IActionResult> Statistics()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var stats = await _paymentService.GetPaymentStatisticsAsync(userId);
            return View(stats);
        }

        // GET: /Payment/ExportCSV
        public async Task<IActionResult> ExportCSV(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var payments = await _paymentService.GetPaymentHistoryAsync(userId, fromDate, toDate);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Mã GD,Số tiền,Phương thức,Trạng thái,Mã GD,Ngày,Thông tin");

            foreach (var p in payments)
            {
                csv.AppendLine($"{p.Id},{p.Amount:N0},{p.Method},{p.Status},{p.TransactionId},{p.CreatedAt:dd/MM/yyyy HH:mm},{p.PaymentInfo}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"payment_history_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}