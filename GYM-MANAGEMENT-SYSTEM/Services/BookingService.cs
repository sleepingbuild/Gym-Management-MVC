using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITrainerRepository _trainerRepository;
        private readonly ITrainerScheduleRepository _scheduleRepository;

        public BookingService(
            IBookingRepository bookingRepository,
            ITrainerRepository trainerRepository,
            ITrainerScheduleRepository scheduleRepository)
        {
            _bookingRepository = bookingRepository;
            _trainerRepository = trainerRepository;
            _scheduleRepository = scheduleRepository;
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
            // Kiểm tra trainer tồn tại
            var trainer = await _trainerRepository.GetByIdAsync(model.TrainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }

            // Kiểm tra trainer có đang hoạt động không
            if (!trainer.IsAvailable)
            {
                throw new InvalidOperationException("Huấn luyện viên này hiện không hoạt động.");
            }

            // Kiểm tra slot có trống không (với chính trainer này)
            if (!await _bookingRepository.IsSlotAvailableAsync(model.TrainerId, model.SessionDate, model.TimeSlot))
            {
                throw new InvalidOperationException("Khung giờ này đã được đặt. Vui lòng chọn khung giờ khác.");
            }

            // Kiểm tra học viên đã có lịch nào khác (với HLV khác) trùng đúng
            // ngày + khung giờ này chưa — 1 người không thể tập 2 chỗ cùng lúc.
            var userBookings = await _bookingRepository.GetByUserIdAsync(model.UserId);
            var hasConflict = userBookings.Any(b =>
                b.SessionDate.Date == model.SessionDate.Date &&
                b.TimeSlot == model.TimeSlot &&
                b.Status != "Cancelled");

            if (hasConflict)
            {
                throw new InvalidOperationException("Bạn đã có một lịch tập khác vào cùng ngày và khung giờ này. Vui lòng chọn khung giờ khác.");
            }

            // ============================================================
            // TẠM THỜI VÔ HIỆU HÓA — kiểm tra lịch làm việc (TrainerSchedule)
            // Lý do: dữ liệu TrainerSchedule hiện chưa được khai báo đầy đủ nên
            // hầu như huấn luyện viên nào cũng bị chặn "không làm việc ngày nào
            // cả", khiến không test được luồng đặt lịch. Mở lại đoạn dưới đây
            // (bỏ /* */) sau khi đã nhập đủ TrainerSchedule cho từng trainer.
            // ============================================================
            /*
            // Kiểm tra trainer có lịch làm việc vào ngày này không
            var dayOfWeek = model.SessionDate.DayOfWeek;
            var timeOnly = TimeOnly.FromDateTime(model.SessionDate);
            var schedules = await _scheduleRepository.GetAvailableSlotsAsync(model.TrainerId, model.SessionDate);

            if (!schedules.Any())
            {
                throw new InvalidOperationException("Huấn luyện viên không làm việc vào ngày này.");
            }

            // Kiểm tra thời gian nằm trong khung làm việc
            var isValidSlot = schedules.Any(s =>
                timeOnly >= s.StartTime && timeOnly < s.EndTime);

            if (!isValidSlot)
            {
                throw new InvalidOperationException("Khung giờ này không nằm trong lịch làm việc của huấn luyện viên.");
            }
            */

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

            // Lọc theo từ khóa (tìm trong Notes hoặc Trainer Name)
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
    }
}