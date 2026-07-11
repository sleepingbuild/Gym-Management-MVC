using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IPaymentService
    {
        // Payment CRUD
        Task<Payment> CreatePaymentAsync(PaymentCreateViewModel model);
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId);
        Task<Payment> UpdatePaymentStatusAsync(int id, string status, string transactionId = "");
        Task<IEnumerable<Payment>> GetPaymentsByMembershipAsync(int membershipId);

        // VNPay
        Task<bool> ProcessVNPayReturn(Dictionary<string, string> response);
        Task<string> CreateVNPayPaymentUrl(int membershipId, string ipAddress);

        // Payment History
        Task<IEnumerable<Payment>> GetPaymentHistoryAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<Payment>> GetPaymentHistoryByStatusAsync(string userId, string status);
        Task<IEnumerable<Payment>> SearchPaymentsAsync(string userId, string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<PaymentStatisticsViewModel> GetPaymentStatisticsAsync(string userId);
    }
}