using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using GYM_MANAGEMENT_SYSTEM.VNPay;
using Microsoft.Extensions.Options;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMembershipPackageRepository _packageRepository;
        private readonly VNPayConfig _vnpayConfig;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMembershipRepository membershipRepository,
            IMembershipPackageRepository packageRepository,
            IOptions<VNPayConfig> vnpayConfig)
        {
            _paymentRepository = paymentRepository;
            _membershipRepository = membershipRepository;
            _packageRepository = packageRepository;
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

        public async Task<bool> ProcessVNPayReturn(Dictionary<string, string> response)
        {

            response.TryGetValue("vnp_TransactionNo", out var transactionId);
            response.TryGetValue("vnp_ResponseCode", out var responseCode);
            response.TryGetValue("vnp_OrderInfo", out var orderInfo);
            response.TryGetValue("vnp_Amount", out var amountStr);
            response.TryGetValue("vnp_PayDate", out var payDate);

      
            var membershipId = 0;
            if (!string.IsNullOrEmpty(orderInfo))
            {
                var parts = orderInfo.Split('-');
                if (parts.Length > 0 && int.TryParse(parts[0], out var id))
                {
                    membershipId = id;
                }
            }

  
            if (responseCode == "00") 
            {

                var payments = await _paymentRepository.GetByMembershipIdAsync(membershipId);
                var pendingPayment = payments.FirstOrDefault(p => p.Status == "Pending");

                if (pendingPayment != null)
                {
                    pendingPayment.Status = "Success";
                    pendingPayment.TransactionId = transactionId ?? "";
                    pendingPayment.PaymentInfo = $"VNPay - {payDate}";
                    await _paymentRepository.UpdateAsync(pendingPayment);

   
                    var membership = await _membershipRepository.GetByIdAsync(membershipId);
                    if (membership != null)
                    {
                        membership.Status = "Active";
                        await _membershipRepository.UpdateAsync(membership);
                    }

                    return true;
                }
            }
            else 
            {
                var payments = await _paymentRepository.GetByMembershipIdAsync(membershipId);
                var pendingPayment = payments.FirstOrDefault(p => p.Status == "Pending");

                if (pendingPayment != null)
                {
                    pendingPayment.Status = "Failed";
                    pendingPayment.PaymentInfo = $"VNPay Error: {responseCode}";
                    await _paymentRepository.UpdateAsync(pendingPayment);
                }
            }

            return false;
        }

        public async Task<string> CreateVNPayPaymentUrl(int membershipId, string ipAddress)
        {
            var membership = await _membershipRepository.GetByIdAsync(membershipId);
            if (membership == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            var package = await _packageRepository.GetByIdAsync(membership.MembershipPackageId);
            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }
         var payment = await CreatePaymentAsync(new PaymentCreateViewModel
            {
                UserId = membership.UserId,
                MembershipId = membershipId,
                Amount = package.Price,
                Method = "VNPay",
                PaymentInfo = $"Thanh toán gói {package.Name} - {membershipId}"
            });

        
            var vnpay = new VNPayRequest(_vnpayConfig);
            vnpay.AddParameter("vnp_Amount", (package.Price * 100).ToString());
            vnpay.AddParameter("vnp_OrderInfo", $"{membershipId}-{payment.Id}");
            vnpay.AddParameter("vnp_OrderType", "other");
            vnpay.AddParameter("vnp_TxnRef", $"{DateTime.Now.Ticks}");
            vnpay.AddParameter("vnp_TransactionNo", payment.Id.ToString());

            return vnpay.CreatePaymentUrl(ipAddress);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByMembershipAsync(int membershipId)
        {
            return await _paymentRepository.GetByMembershipIdAsync(membershipId);
        }
    }
}