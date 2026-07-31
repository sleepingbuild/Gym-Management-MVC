using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Booking>> GetByTrainerIdAsync(int trainerId);
        Task<IEnumerable<Booking>> GetByDateAsync(DateTime date);
        Task<IEnumerable<Booking>> GetByUserAndDateAsync(string userId, DateTime date);
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking> AddAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsSlotAvailableAsync(int trainerId, DateTime sessionDate, string timeSlot, int? excludeId = null);
        Task<int> CountBookingsForTrainerAsync(int trainerId, DateTime date);
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(string userId);
        Task<Booking?> GetTodayBookingForUserAsync(string userId, DateTime date);
    }
}