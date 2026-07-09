using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Payment>> GetByMembershipIdAsync(int membershipId);
        Task<Payment?> GetByIdAsync(int id);
        Task<Payment> AddAsync(Payment payment);
        Task<Payment> UpdateAsync(Payment payment);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}