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

            // Kiểm tra slot có trống không
            if (!await _bookingRepository.IsSlotAvailableAsync(model.TrainerId, model.SessionDate, model.TimeSlot))
            {
                throw new InvalidOperationException("Khung giờ này đã được đặt. Vui lòng chọn khung giờ khác.");
            }

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
    }
}