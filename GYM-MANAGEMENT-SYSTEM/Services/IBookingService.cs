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
        Task<IEnumerable<Booking>> GetBookingHistoryAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<Booking>> GetBookingHistoryByStatusAsync(string userId, string status);
        Task<IEnumerable<Booking>> SearchBookingsAsync(string userId, string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<BookingStatisticsViewModel> GetBookingStatisticsAsync(string userId);
        Task<Booking> CreateBookingByAdminAsync(AdminBookingCreateViewModel model);

        Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate, int? trainerId = null);

        Task<IEnumerable<BookableMemberViewModel>> GetBookableMembersAsync();

        Task<Dictionary<string, (string FullName, string Email)>> GetMemberDisplayInfoAsync(IEnumerable<string> userIds);

        IEnumerable<string> GetFixedTimeSlots();
    }
}