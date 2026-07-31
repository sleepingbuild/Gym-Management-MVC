using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Trainer)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Trainer)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByTrainerIdAsync(int trainerId)
        {
            return await _context.Bookings
                .Include(b => b.Trainer)
                .Where(b => b.TrainerId == trainerId)
                .OrderByDescending(b => b.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            return await _context.Bookings
                .Include(b => b.Trainer)
                .Where(b => b.SessionDate >= startDate && b.SessionDate < endDate)
                .OrderBy(b => b.SessionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByUserAndDateAsync(string userId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            return await _context.Bookings
                .Include(b => b.Trainer)
                .Where(b => b.UserId == userId && b.SessionDate >= startDate && b.SessionDate < endDate)
                .OrderBy(b => b.SessionDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Trainer)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            booking.CreatedAt = DateTime.UtcNow;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await GetByIdAsync(id);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsSlotAvailableAsync(int trainerId, DateTime sessionDate, string timeSlot, int? excludeId = null)
        {
            var query = _context.Bookings
                .Where(b => b.TrainerId == trainerId
                           && b.SessionDate.Date == sessionDate.Date
                           && b.TimeSlot == timeSlot
                           && b.Status != "Cancelled");

            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<int> CountBookingsForTrainerAsync(int trainerId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            return await _context.Bookings
                .Where(b => b.TrainerId == trainerId
                           && b.SessionDate >= startDate
                           && b.SessionDate < endDate
                           && b.Status != "Cancelled")
                .CountAsync();
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Bookings
                .Include(b => b.Trainer)
                .Where(b => b.UserId == userId
                           && b.SessionDate >= today
                           && b.Status != "Cancelled")
                .OrderBy(b => b.SessionDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetTodayBookingForUserAsync(string userId, DateTime date)
        {
            var dateOnly = date.Date;
            return await _context.Bookings
                .Include(b => b.Trainer)
                .Where(b => b.UserId == userId
                         && b.SessionDate.Date == dateOnly
                         && (b.Status == "Pending" || b.Status == "Confirmed")
                         && b.CheckInTime == null)
                .OrderBy(b => b.TimeSlot)
                .FirstOrDefaultAsync();
        }
    }
}