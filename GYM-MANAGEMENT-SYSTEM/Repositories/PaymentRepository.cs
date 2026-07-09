using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Membership)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByUserIdAsync(string userId)
        {
            return await _context.Payments
                .Include(p => p.Membership)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByMembershipIdAsync(int membershipId)
        {
            return await _context.Payments
                .Where(p => p.MembershipId == membershipId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            return await _context.Payments
                .Include(p => p.Membership)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            payment.CreatedAt = DateTime.UtcNow;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await GetByIdAsync(id);
            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Payments
                .Where(p => p.CreatedAt >= fromDate && p.CreatedAt <= toDate)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Payments.Where(p => p.Status == "Success");

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= toDate.Value);
            }

            return await query.SumAsync(p => p.Amount);
        }
    }
}