using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId);
        Task<IEnumerable<Booking>> GetTrainerBookingsAsync(int trainerId);
        Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<Booking> CreateBookingAsync(BookingCreateViewModel model);
        Task<Booking> UpdateBookingAsync(BookingEditViewModel model);
        Task<bool> CancelBookingAsync(int id);
        Task<bool> ConfirmBookingAsync(int id);
        Task<bool> CompleteBookingAsync(int id);
        Task<bool> IsSlotAvailableAsync(int trainerId, DateTime sessionDate, string timeSlot, int? excludeId = null);
        Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(string userId);
        Task<int> GetBookingCountForTrainerAsync(int trainerId, DateTime date);
    }
}