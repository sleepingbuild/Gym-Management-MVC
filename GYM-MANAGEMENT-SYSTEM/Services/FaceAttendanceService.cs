using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class FaceAttendanceService : IFaceAttendanceService
    {
        private readonly ITrainerRepository _trainerRepository;
        private readonly ITrainerAttendanceService _trainerAttendanceService;
        private readonly IBookingRepository _bookingRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public FaceAttendanceService(
            ITrainerRepository trainerRepository,
            ITrainerAttendanceService trainerAttendanceService,
            IBookingRepository bookingRepository,
            UserManager<ApplicationUser> userManager)
        {
            _trainerRepository = trainerRepository;
            _trainerAttendanceService = trainerAttendanceService;
            _bookingRepository = bookingRepository;
            _userManager = userManager;
        }

        public async Task<FaceCheckInResultViewModel> CheckInAsync(string userId)
        {
            var today = DateTime.UtcNow.Date;

            // 1) Thử tìm xem người này có phải là Trainer không
            var trainer = await _trainerRepository.GetByUserIdAsync(userId);
            if (trainer != null)
            {
                return await CheckInTrainerAsync(trainer);
            }

            // 2) Không phải Trainer -> xử lý như Member, cần có Booking hôm nay
            return await CheckInMemberAsync(userId, today);
        }

        private async Task<FaceCheckInResultViewModel> CheckInTrainerAsync(Trainer trainer)
        {
            try
            {
                await _trainerAttendanceService.CheckInAsync(trainer.Id, "Điểm danh bằng khuôn mặt", "Face");

                return new FaceCheckInResultViewModel
                {
                    Success = true,
                    Message = $"Điểm danh HLV thành công lúc {DateTime.Now:HH:mm}",
                    FullName = trainer.FullName,
                    Role = "Trainer",
                    Time = DateTime.UtcNow
                };
            }
            catch (InvalidOperationException ex)
            {
                // Ví dụ: đã chấm công hôm nay rồi
                return new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = ex.Message,
                    FullName = trainer.FullName,
                    Role = "Trainer"
                };
            }
        }

        private async Task<FaceCheckInResultViewModel> CheckInMemberAsync(string userId, DateTime today)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var fullName = user?.FullName ?? "N/A";

            var booking = await _bookingRepository.GetTodayBookingForUserAsync(userId, today);
            if (booking == null)
            {
                return new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = "Không tìm thấy lịch đặt hôm nay. Vui lòng đặt lịch trước khi điểm danh.",
                    FullName = fullName,
                    Role = "Member"
                };
            }

            booking.CheckInTime = DateTime.UtcNow;
            booking.CheckInMethod = "Face";
            await _bookingRepository.UpdateAsync(booking);

            return new FaceCheckInResultViewModel
            {
                Success = true,
                Message = $"Điểm danh buổi tập lúc {booking.TimeSlot} thành công!",
                FullName = fullName,
                Role = "Member",
                Time = DateTime.UtcNow
            };
        }
    }
}