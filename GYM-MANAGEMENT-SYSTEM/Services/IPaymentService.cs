using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(PaymentCreateViewModel model);
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetUserPaymentsAsync(string userId);
        Task<Payment> UpdatePaymentStatusAsync(int id, string status, string transactionId = "");
        Task<bool> ProcessVNPayReturn(Dictionary<string, string> response);
        Task<string> CreateVNPayPaymentUrl(int membershipId, string ipAddress);
        Task<IEnumerable<Payment>> GetPaymentsByMembershipAsync(int membershipId);
    }
}