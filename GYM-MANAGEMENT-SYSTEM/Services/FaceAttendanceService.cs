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

        public async Task<FaceCheckInResultViewModel> ProcessScanAsync(string userId)
        {
            var today = DateTime.Now.Date;

            // 1) Thử tìm xem người này có phải là Trainer không
            var trainer = await _trainerRepository.GetByUserIdAsync(userId);
            if (trainer != null)
            {
                return await ProcessTrainerScanAsync(trainer);
            }

            // 2) Không phải Trainer -> xử lý như Member
            return await ProcessMemberScanAsync(userId, today);
        }

        // ===================== Trainer =====================

        private async Task<FaceCheckInResultViewModel> ProcessTrainerScanAsync(Trainer trainer)
        {
            var todayRecord = await _trainerAttendanceService.GetTodayRecordAsync(trainer.Id);

            
            if (todayRecord == null)
            {
                return await TrainerCheckInAsync(trainer);
            }

            if (todayRecord.CheckOutTime == null)
            {
                return await TrainerCheckOutAsync(trainer);
            }

            return new FaceCheckInResultViewModel
            {
                Success = false,
                Message = "Bạn đã điểm danh đủ vào ca và tan ca cho hôm nay.",
                FullName = trainer.FullName,
                Role = "Trainer"
            };
        }

        private async Task<FaceCheckInResultViewModel> TrainerCheckInAsync(Trainer trainer)
        {
            try
            {
                await _trainerAttendanceService.CheckInAsync(trainer.Id, "Điểm danh bằng khuôn mặt", "Face");

                var now = DateTime.Now;
                var message = $"Điểm danh VÀO CA thành công lúc {now:HH:mm}";

                if (trainer.ShiftStartTime.HasValue)
                {
                    var lateMinutes = (int)(TimeOnly.FromDateTime(now).ToTimeSpan() - trainer.ShiftStartTime.Value.ToTimeSpan()).TotalMinutes;
                    message += lateMinutes > 0
                        ? $" (đi muộn {lateMinutes} phút so với ca {trainer.ShiftStartTime:HH\\:mm})"
                        : $" (đúng giờ — ca bắt đầu {trainer.ShiftStartTime:HH\\:mm})";
                }

                return new FaceCheckInResultViewModel
                {
                    Success = true,
                    Message = message,
                    FullName = trainer.FullName,
                    Role = "Trainer",
                    Action = "CheckIn",
                    Time = DateTime.Now
                };
            }
            catch (InvalidOperationException ex)
            {
                return new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = ex.Message,
                    FullName = trainer.FullName,
                    Role = "Trainer"
                };
            }
        }

        private async Task<FaceCheckInResultViewModel> TrainerCheckOutAsync(Trainer trainer)
        {
            try
            {
                await _trainerAttendanceService.CheckOutAsync(trainer.Id);

                var now = DateTime.Now;
                var message = $"Điểm danh TAN CA thành công lúc {now:HH:mm}";

                if (trainer.ShiftEndTime.HasValue)
                {
                    var earlyMinutes = (int)(trainer.ShiftEndTime.Value.ToTimeSpan() - TimeOnly.FromDateTime(now).ToTimeSpan()).TotalMinutes;
                    message += earlyMinutes > 0
                        ? $" (về sớm {earlyMinutes} phút so với ca kết thúc {trainer.ShiftEndTime:HH\\:mm})"
                        : $" (đúng giờ — ca kết thúc {trainer.ShiftEndTime:HH\\:mm})";
                }

                return new FaceCheckInResultViewModel
                {
                    Success = true,
                    Message = message,
                    FullName = trainer.FullName,
                    Role = "Trainer",
                    Action = "CheckOut",
                    Time = DateTime.Now
                };
            }
            catch (InvalidOperationException ex)
            {
                return new FaceCheckInResultViewModel
                {
                    Success = false,
                    Message = ex.Message,
                    FullName = trainer.FullName,
                    Role = "Trainer"
                };
            }
        }

        // ===================== Member =====================

        private async Task<FaceCheckInResultViewModel> ProcessMemberScanAsync(string userId, DateTime today)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var fullName = user?.FullName ?? "N/A";

            var pendingBooking = await _bookingRepository.GetTodayBookingForUserAsync(userId, today);
            if (pendingBooking != null)
            {
                pendingBooking.CheckInTime = DateTime.Now;
                pendingBooking.CheckInMethod = "Face";
                await _bookingRepository.UpdateAsync(pendingBooking);

                return new FaceCheckInResultViewModel
                {
                    Success = true,
                    Message = $"Điểm danh VÀO buổi tập lúc {pendingBooking.TimeSlot} thành công!",
                    FullName = fullName,
                    Role = "Member",
                    Action = "CheckIn",
                    Time = DateTime.Now
                };
            }

            var checkedInBooking = await _bookingRepository.GetTodayCheckedInBookingForUserAsync(userId, today);
            if (checkedInBooking != null)
            {
                checkedInBooking.CheckOutTime = DateTime.Now;
                checkedInBooking.CheckOutMethod = "Face";
                await _bookingRepository.UpdateAsync(checkedInBooking);

                return new FaceCheckInResultViewModel
                {
                    Success = true,
                    Message = $"Điểm danh RA VỀ thành công lúc {DateTime.Now:HH:mm}. Hẹn gặp lại!",
                    FullName = fullName,
                    Role = "Member",
                    Action = "CheckOut",
                    Time = DateTime.Now
                };
            }

            return new FaceCheckInResultViewModel
            {
                Success = false,
                Message = "Không tìm thấy lịch đặt hôm nay cần điểm danh (hoặc đã điểm danh đủ vào/ra).",
                FullName = fullName,
                Role = "Member"
            };
        }
    }
}