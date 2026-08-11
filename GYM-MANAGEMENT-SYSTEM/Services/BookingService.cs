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
        private readonly IPaymentService _paymentService;

        public static readonly TimeOnly WorkingHourStart = new TimeOnly(7, 0);
        public static readonly TimeOnly WorkingHourEnd = new TimeOnly(21, 0);

        public BookingService(
            IBookingRepository bookingRepository,
            ITrainerRepository trainerRepository,
            UserManager<ApplicationUser> userManager,
            IPaymentService paymentService)
        {
            _bookingRepository = bookingRepository;
            _trainerRepository = trainerRepository;
            _userManager = userManager;
            _paymentService = paymentService;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return await AutoCancelExpiredAsync(bookings);
        }

        public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            return await AutoCancelExpiredAsync(bookings);
        }

        public async Task<IEnumerable<Booking>> GetTrainerBookingsAsync(int trainerId)
        {
            var bookings = await _bookingRepository.GetByTrainerIdAsync(trainerId);
            return await AutoCancelExpiredAsync(bookings);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            var bookings = await _bookingRepository.GetByDateAsync(date);
            return await AutoCancelExpiredAsync(bookings);
        }

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking != null)
            {
                await AutoCancelIfExpiredAsync(booking);
            }
            return booking;
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

            if (!await _bookingRepository.IsSlotAvailableAsync(model.TrainerId, model.SessionDate, model.TimeSlot))
            {
                throw new InvalidOperationException("Khung giờ này đã được đặt. Vui lòng chọn khung giờ khác.");
            }

            EnsureWithinWorkingHours(model.TimeSlot, trainer);
            EnsureNotPastOrTooSoon(model.SessionDate, model.TimeSlot);
            await EnsurePackageAllowsBookingAsync(model.UserId, model.SessionDate);
            await EnsureMemberNotDoubleBookedAsync(model.UserId, model.SessionDate, model.TimeSlot);

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

            // Nếu thay đổi thời gian, kiểm tra slot
            if (booking.TrainerId != model.TrainerId ||
                booking.SessionDate != model.SessionDate ||
                booking.TimeSlot != model.TimeSlot)
            {
                if (!await _bookingRepository.IsSlotAvailableAsync(model.TrainerId, model.SessionDate, model.TimeSlot, model.Id))
                {
                    throw new InvalidOperationException("Khung giờ này đã được đặt. Vui lòng chọn khung giờ khác.");
                }

                await EnsurePackageAllowsBookingAsync(booking.UserId, model.SessionDate, model.Id);
                await EnsureMemberNotDoubleBookedAsync(booking.UserId, model.SessionDate, model.TimeSlot, model.Id);
            }

            booking.TrainerId = model.TrainerId;
            booking.SessionDate = model.SessionDate;
            booking.TimeSlot = model.TimeSlot;
            booking.Notes = model.Notes;

            return await _bookingRepository.UpdateAsync(booking);
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
            if (booking == null)
                return false;

            booking.Status = "Confirmed";
            await _bookingRepository.UpdateAsync(booking);
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
            return await _bookingRepository.IsSlotAvailableAsync(trainerId, sessionDate, timeSlot, excludeId);
        }

        public async Task<IEnumerable<Booking>> GetUpcomingBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetUpcomingBookingsAsync(userId);
            return await AutoCancelExpiredAsync(bookings);
        }

        public async Task<int> GetBookingCountForTrainerAsync(int trainerId, DateTime date)
        {
            return await _bookingRepository.CountBookingsForTrainerAsync(trainerId, date);
        }

        public async Task<IEnumerable<Booking>> GetBookingHistoryAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            bookings = await AutoCancelExpiredAsync(bookings);

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
            bookings = await AutoCancelExpiredAsync(bookings);
            return bookings.Where(b => b.Status == status)
                           .OrderByDescending(b => b.SessionDate);
        }

        public async Task<IEnumerable<Booking>> SearchBookingsAsync(string userId, string? searchTerm = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var bookings = await _bookingRepository.GetByUserIdAsync(userId);
            bookings = await AutoCancelExpiredAsync(bookings);

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
            bookings = await AutoCancelExpiredAsync(bookings);

            var stats = new BookingStatisticsViewModel
            {
                TotalBookings = bookings.Count(),
                PendingBookings = bookings.Count(b => b.Status == "Pending"),
                ConfirmedBookings = bookings.Count(b => b.Status == "Confirmed"),
                CompletedBookings = bookings.Count(b => b.Status == "Completed"),
                CancelledBookings = bookings.Count(b => b.Status == "Cancelled"),
                NoShowBookings = bookings.Count(b => b.Status == "NoShow"),
                PtNoShowBookings = bookings.Count(b => b.Status == "PtNoShow"),
                UpcomingBookings = bookings.Count(b => b.SessionDate >= DateTime.UtcNow && b.Status != "Cancelled"),
                PastBookings = bookings.Count(b => b.SessionDate < DateTime.UtcNow)
            };

            // Tính tổng số buổi đã hoàn thành (gần đây)
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

        // ===================== Admin đặt lịch hộ Member =====================

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

            EnsureNotPastOrTooSoon(model.SessionDate, model.TimeSlot);
            await EnsurePackageAllowsBookingAsync(model.UserId, model.SessionDate);
            await EnsureMemberNotDoubleBookedAsync(model.UserId, model.SessionDate, model.TimeSlot);

            if (!await _bookingRepository.IsSlotAvailableAsync(model.TrainerId, model.SessionDate, model.TimeSlot))
            {
                throw new InvalidOperationException("Khung giờ này đã được đặt. Vui lòng chọn khung giờ khác.");
            }

            EnsureWithinWorkingHours(model.TimeSlot, trainer);

            var booking = new Booking
            {
                UserId = model.UserId,
                TrainerId = model.TrainerId,
                SessionDate = model.SessionDate,
                TimeSlot = model.TimeSlot,
                // Admin đặt hộ -> bỏ qua bước Trainer xác nhận, Confirmed ngay.
                Status = "Confirmed",
                Notes = model.Notes,
                CreatedAt = DateTime.UtcNow
            };

            return await _bookingRepository.AddAsync(booking);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateRangeAsync(DateOnly startDate, DateOnly endDate, int? trainerId = null)
        {
            var bookings = await _bookingRepository.GetByDateRangeAsync(startDate, endDate, trainerId);
            return await AutoCancelExpiredAsync(bookings);
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
            return BuildSlots(WorkingHourStart, WorkingHourEnd);
        }

        public async Task<IEnumerable<string>> GetTimeSlotsForTrainerAsync(int trainerId)
        {
            var trainer = await _trainerRepository.GetByIdAsync(trainerId);
            var start = trainer?.ShiftStartTime ?? WorkingHourStart;
            var end = trainer?.ShiftEndTime ?? WorkingHourEnd;
            return BuildSlots(start, end);
        }

        private static IEnumerable<string> BuildSlots(TimeOnly start, TimeOnly end)
        {
            var slots = new List<string>();
            for (var hour = start.Hour; hour < end.Hour; hour++)
            {
                slots.Add($"{hour:00}:00");
            }
            return slots;
        }

        public const int MinMinutesAhead = 30;

       
        private async Task EnsurePackageAllowsBookingAsync(string userId, DateTime sessionDate, int? excludeBookingId = null)
        {
            var currentMembership = await _paymentService.GetCurrentMembershipAsync(userId);
            var maxPerWeek = currentMembership?.MembershipPackage?.MaxSessionsPerWeek;

            // Gói không giới hạn (hoặc chưa xác định được gói hiện tại) -> không chặn thêm.
            if (!maxPerWeek.HasValue)
            {
                return;
            }

            if (maxPerWeek.Value == 0)
            {
                throw new InvalidOperationException(
                    "Gói tập hiện tại của bạn không bao gồm đặt lịch với huấn luyện viên. Vui lòng nâng cấp gói để sử dụng tính năng này.");
            }

            var diff = (7 + (sessionDate.Date.DayOfWeek - DayOfWeek.Monday)) % 7;
            var weekStart = sessionDate.Date.AddDays(-diff);
            var weekEnd = weekStart.AddDays(6);

            var userBookings = await _bookingRepository.GetByUserIdAsync(userId);
            var countThisWeek = userBookings.Count(b =>
                b.Id != excludeBookingId &&
                b.SessionDate.Date >= weekStart && b.SessionDate.Date <= weekEnd &&
                (b.Status == "Pending" || b.Status == "Confirmed" || b.Status == "Completed"));

            if (countThisWeek >= maxPerWeek.Value)
            {
                throw new InvalidOperationException(
                    $"Gói tập hiện tại chỉ cho phép đặt tối đa {maxPerWeek.Value} buổi/tuần với huấn luyện viên. Bạn đã đặt đủ số buổi trong tuần này.");
            }
        }

        private async Task EnsureMemberNotDoubleBookedAsync(string userId, DateTime sessionDate, string timeSlot, int? excludeBookingId = null)
        {
            var sameDayBookings = await _bookingRepository.GetByDateAsync(sessionDate);

            var alreadyBooked = sameDayBookings.Any(b =>
                b.Id != excludeBookingId &&
                b.UserId == userId &&
                b.TimeSlot == timeSlot &&
                (b.Status == "Pending" || b.Status == "Confirmed"));

            if (alreadyBooked)
            {
                throw new InvalidOperationException(
                    "Bạn đã có lịch tập vào khung giờ này rồi (với huấn luyện viên khác). Vui lòng chọn khung giờ khác.");
            }
        }


        private async Task AutoCancelIfExpiredAsync(Booking booking)
        {
            if (booking.Status != "Pending" && booking.Status != "Confirmed")
            {
                return;
            }

            var (start, end) = ParseTimeSlot(booking.TimeSlot);
            var sessionStart = booking.SessionDate.Date + start.ToTimeSpan();
            var sessionEnd = booking.SessionDate.Date + end.ToTimeSpan();
            var now = DateTime.Now;

            if (booking.Status == "Pending" && now < sessionStart && (sessionStart - now) <= TimeSpan.FromHours(1))
            {
                booking.Status = "Cancelled";
                var autoCancelNote = "[Tự động huỷ do chưa được xác nhận trước giờ hẹn 1 tiếng]";
                booking.Notes = string.IsNullOrWhiteSpace(booking.Notes) ? autoCancelNote : $"{booking.Notes} {autoCancelNote}";
                await _bookingRepository.UpdateAsync(booking);
                return;
            }

            if (booking.Status == "Confirmed" && now >= sessionStart.AddMinutes(30))
            {
                booking.Status = "PtNoShow";
                var autoPtNote = "[Tự động đánh dấu PT không đến sau 30 phút kể từ giờ hẹn]";
                booking.Notes = string.IsNullOrWhiteSpace(booking.Notes) ? autoPtNote : $"{booking.Notes} {autoPtNote}";
                await _bookingRepository.UpdateAsync(booking);
                return;
            }

            if (booking.CheckInTime.HasValue)
            {
                return;
            }

            if (sessionEnd <= now)
            {
                booking.Status = "NoShow";
                var autoNoShowNote = "[Tự động đánh dấu Không đến do quá giờ tập mà học viên không điểm danh]";
                booking.Notes = string.IsNullOrWhiteSpace(booking.Notes) ? autoNoShowNote : $"{booking.Notes} {autoNoShowNote}";
                await _bookingRepository.UpdateAsync(booking);
            }
        }

        private async Task<IEnumerable<Booking>> AutoCancelExpiredAsync(IEnumerable<Booking> bookings)
        {
            var list = bookings.ToList();
            foreach (var booking in list)
            {
                await AutoCancelIfExpiredAsync(booking);
            }
            return list;
        }

        private static void EnsureNotPastOrTooSoon(DateTime sessionDate, string timeSlot)
        {
            if (sessionDate.Date != DateTime.Today)
            {
                return;
            }

            var (start, _) = ParseTimeSlot(timeSlot);
            var slotDateTime = sessionDate.Date + start.ToTimeSpan();
            var minAllowed = DateTime.Now.AddMinutes(MinMinutesAhead);

            if (slotDateTime < minAllowed)
            {
                throw new InvalidOperationException(
                    $"Khung giờ này đã qua hoặc quá gần thời điểm hiện tại. Vui lòng đặt trước ít nhất {MinMinutesAhead} phút (từ {minAllowed:HH:mm} trở đi).");
            }
        }

        private static void EnsureWithinWorkingHours(string timeSlot, Trainer trainer)
        {
            var (start, end) = ParseTimeSlot(timeSlot);

            var shiftStart = trainer.ShiftStartTime ?? WorkingHourStart;
            var shiftEnd = trainer.ShiftEndTime ?? WorkingHourEnd;

            if (start < shiftStart || end > shiftEnd)
            {
                throw new InvalidOperationException(
                    $"Huấn luyện viên {trainer.FullName} chỉ nhận lịch trong ca làm việc {shiftStart:HH:mm}–{shiftEnd:HH:mm}. Vui lòng chọn khung giờ khác.");
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