using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITrainerRepository _trainerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public static readonly TimeOnly WorkingHourStart = new TimeOnly(7, 0);
        public static readonly TimeOnly WorkingHourEnd = new TimeOnly(21, 0);

        public BookingService(
            IBookingRepository bookingRepository,
            ITrainerRepository trainerRepository,
            UserManager<ApplicationUser> userManager)
        {
            _bookingRepository = bookingRepository;
            _trainerRepository = trainerRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _bookingRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Booking>> GetTrainerBookingsAsync(int trainerId)
        {
            return await _bookingRepository.GetByTrainerIdAsync(trainerId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _bookingRepository.GetByDateAsync(date);
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<Booking> CreateBookingAsync(BookingCreateViewModel model)
        {
            var trainer = await _trainerRepository.GetByIdAsync(model.TrainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }

            if (!trainer.IsAvailable)
            {
                throw new InvalidOperationException("Huấn luyện viên này hiện không hoạt động.");
            }

            EnsureWithinWorkingHours(model.TimeSlot);

            // Thay cho IsSlotAvailableAsync cũ (chỉ cho 1 người/slot)
            await EnsureSlotHasCapacityAsync(model.TrainerId, model.SessionDate, model.TimeSlot);

            var booking = new Booking
            {
                UserId = model.UserId,
                TrainerId = model.TrainerId,
                SessionDate = model.SessionDate,
                TimeSlot = model.TimeSlot,
                Status = "Pending",
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow
            };

            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<Booking> UpdateBookingAsync(BookingEditViewModel model)
        {
            var booking = await _bookingRepository.GetByIdAsync(model.Id);
            if (booking == null)
            {
                throw new KeyNotFoundException("Không tìm thấy booking.");
            }

            if (booking.TrainerId != model.TrainerId ||
                booking.SessionDate != model.SessionDate ||
                booking.TimeSlot != model.TimeSlot)
            {
                await EnsureSlotHasCapacityAsync(model.TrainerId, model.SessionDate, model.TimeSlot, model.Id);
            }

            booking.TrainerId = model.TrainerId;
            booking.SessionDate = model.SessionDate;
            booking.TimeSlot = model.TimeSlot;
            booking.Notes = model.Notes;

            return await _bookingRepository.UpdateAsync(booking);
        }

        public const int MaxBookingsPerSlot = 2;

        private async Task EnsureSlotHasCapacityAsync(int trainerId, DateTime sessionDate, string timeSlot, int? excludeBookingId = null)
        {
            var sameDayBookings = await _bookingRepository.GetByDateAsync(sessionDate);

            var activeCount = sameDayBookings.Count(b =>
                b.Id != excludeBookingId &&
                b.TrainerId == trainerId &&
                b.TimeSlot == timeSlot &&
                (b.Status == "Pending" || b.Status == "Confirmed"));

            if (activeCount >= MaxBookingsPerSlot)
            {
                throw new InvalidOperationException(
                    "Khung giờ này đã đủ số lượng đặt lịch (tối đa 2 người/slot). Vui lòng chọn khung giờ khác.");
            }
        }

        public async Task<bool> CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return false;

            booking.Status = "Cancelled";
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> ConfirmBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null || booking.Status != "Pending")
            {
                return false;
            }

            booking.Status = "Confirmed";
            await _bookingRepository.UpdateAsync(booking);

            var sameDayBookings = await _bookingRepository.GetByDateAsync(booking.SessionDate);
            var conflicting = sameDayBookings.Where(b =>
                b.Id != booking.Id &&
                b.TrainerId == booking.TrainerId &&
                b.TimeSlot == booking.TimeSlot &&
                b.Status == "Pending");

            foreach (var conflict in conflicting)
            {
                conflict.Status = "Cancelled";
                await _bookingRepository.UpdateAsync(conflict);
            }

            return true;
        }

        public async Task<bool> CompleteBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return false;

            booking.Status = "Completed";
            await _bookingRepository.UpdateAsync(booking);
            return true;
        }

        public async Task<bool> IsSlotAvailableAsync(int trainerId, DateTime sessionDate, string timeSlot, int? excludeId = null)
        {
            var sameDayBookings = await _bookingRepository.GetByDateAsync(sessionDate);
            var activeCount = sameDayBookings.Count(b =>
                b.Id != excludeId &&
                b.TrainerId == trainerId &&
                b.TimeSlot == timeSlot &&
                (b.Status == "Pending" || b.Status == "Confirmed"));

            return activeCount < MaxBookingsPerSlot;
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(string userId)
        {
            return await _bookingRepository.GetUpcomingBookingsAsync(userId);
        }

        public async Task<int> GetBookingCountForTrainerAsync(int trainerId, DateTime date)
        {
            return await _bookingRepository.CountBookingsForTrainerAsync(trainerId, date);
        }

        public async Task<IEnumerable<Booking>> GetBookingHistoryAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);

            if (fromDate.HasValue)
            {
                bookings = bookings.Where(b => b.SessionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                bookings = bookings.Where(b => b.SessionDate <= toDate.Value);
            }

            return bookings.OrderByDescending(b => b.SessionDate);
        }

        public async Task<IEnumerable<Booking>> GetBookingHistoryByStatusAsync(string userId, string status)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return bookings.Where(b => b.Status == status)
                           .OrderByDescending(b => b.SessionDate);
        }

        public async Task<IEnumerable<Booking>> SearchBookingsAsync(string userId, string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                bookings = bookings.Where(b =>
                    b.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (b.Trainer != null && b.Trainer.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (fromDate.HasValue)
            {
                bookings = bookings.Where(b => b.SessionDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                bookings = bookings.Where(b => b.SessionDate <= toDate.Value);
            }

            return bookings.OrderByDescending(b => b.SessionDate);
        }

        public async Task<BookingStatisticsViewModel> GetBookingStatisticsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);

            var stats = new BookingStatisticsViewModel
            {
                TotalBookings = bookings.Count(),
                PendingBookings = bookings.Count(b => b.Status == "Pending"),
                ConfirmedBookings = bookings.Count(b => b.Status == "Confirmed"),
                CompletedBookings = bookings.Count(b => b.Status == "Completed"),
                CancelledBookings = bookings.Count(b => b.Status == "Cancelled"),
                UpcomingBookings = bookings.Count(b => b.SessionDate >= DateTime.UtcNow && b.Status != "Cancelled"),
                PastBookings = bookings.Count(b => b.SessionDate < DateTime.UtcNow)
            };

            stats.RecentCompleted = bookings
                .Where(b => b.Status == "Completed")
                .OrderByDescending(b => b.SessionDate)
                .Take(5)
                .Select(b => new BookingSummaryViewModel
                {
                    Id = b.Id,
                    TrainerName = b.Trainer?.FullName ?? "N/A",
                    SessionDate = b.SessionDate,
                    Status = b.Status
                }).ToList();

            return stats;
        }


        public async Task<Booking> CreateBookingByAdminAsync(AdminBookingCreateViewModel model)
        {
            var trainer = await _trainerRepository.GetByIdAsync(model.TrainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }

            if (!trainer.IsAvailable)
            {
                throw new InvalidOperationException("Huấn luyện viên này hiện không hoạt động.");
            }

            var member = await _userManager.FindByIdAsync(model.UserId);
            if (member == null)
            {
                throw new KeyNotFoundException("Không tìm thấy thành viên.");
            }

            if (model.SessionDate.Date < DateTime.Today)
            {
                throw new InvalidOperationException("Không thể đặt lịch cho ngày trong quá khứ.");
            }

            await EnsureSlotHasCapacityAsync(model.TrainerId, model.SessionDate, model.TimeSlot);

            EnsureWithinWorkingHours(model.TimeSlot);

            var booking = new Booking
            {
                UserId = model.UserId,
                TrainerId = model.TrainerId,
                SessionDate = model.SessionDate,
                TimeSlot = model.TimeSlot,
                Status = "Confirmed",
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow
            };

            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate, int? trainerId = null)
        {
            return await _bookingRepository.GetByDateRangeAsync(startDate, endDate, trainerId);
        }

        public async Task<IEnumerable<BookableMemberViewModel>> GetBookableMembersAsync()
        {
            var members = await _userManager.GetUsersInRoleAsync("Member");

            return members
                .OrderBy(u => u.FullName)
                .Select(u => new BookableMemberViewModel
                {
                    UserId = u.Id,
                    FullName = string.IsNullOrWhiteSpace(u.FullName) ? (u.Email ?? u.Id) : u.FullName,
                    Email = u.Email ?? string.Empty
                });
        }

        public async Task<Dictionary<string, (string FullName, string Email)>> GetMemberDisplayInfoAsync(IEnumerable<string> userIds)
        {
            var idSet = userIds.ToHashSet();

            var users = _userManager.Users
                .Where(u => idSet.Contains(u.Id))
                .ToList();

            return users.ToDictionary(
                u => u.Id,
                u => (FullName: string.IsNullOrWhiteSpace(u.FullName) ? (u.Email ?? u.Id) : u.FullName, Email: u.Email ?? string.Empty)
            );
        }

        public IEnumerable<string> GetFixedTimeSlots()
        {
            var slots = new List<string>();
            for (var hour = WorkingHourStart.Hour; hour < WorkingHourEnd.Hour; hour++)
            {
                slots.Add($"{hour:00}:00");
            }
            return slots;
        }

        private static void EnsureWithinWorkingHours(string timeSlot)
        {
            var (start, end) = ParseTimeSlot(timeSlot);

            if (start < WorkingHourStart || end > WorkingHourEnd)
            {
                throw new InvalidOperationException(
                    $"Phòng gym chỉ hoạt động từ {WorkingHourStart:HH:mm} đến {WorkingHourEnd:HH:mm}. Vui lòng chọn khung giờ khác.");
            }
        }

        private static (TimeOnly Start, TimeOnly End) ParseTimeSlot(string timeSlot)
        {
            var parts = timeSlot.Split('-');

            if (parts.Length == 2 &&
                TimeOnly.TryParse(parts[0], out var rangeStart) &&
                TimeOnly.TryParse(parts[1], out var rangeEnd))
            {
                return (rangeStart, rangeEnd);
            }

            if (parts.Length == 1 && TimeOnly.TryParse(parts[0], out var singleStart))
            {
                return (singleStart, singleStart.AddHours(1));
            }

            throw new InvalidOperationException("Khung giờ không hợp lệ.");
        }
    }
}