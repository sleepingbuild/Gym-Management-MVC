using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface ITrainerAttendanceService
    {
        Task<TrainerAttendanceStatusViewModel> GetStatusAsync(int trainerId);
        Task CheckInAsync(int trainerId, string? notes, string method = "Manual");

        // Điểm danh tan ca — chỉ hợp lệ nếu đã check-in hôm nay và chưa check-out
        Task<bool> CheckOutAsync(int trainerId);

        // Bản ghi chấm công hôm nay của 1 trainer (null nếu chưa check-in) — dùng để
        // quyết định quét mặt lần này là check-in hay check-out.
        Task<GYM_MANAGEMENT_SYSTEM.Models.TrainerAttendance?> GetTodayRecordAsync(int trainerId);

        Task<AdminAttendanceReportViewModel> GetDailyReportAsync(DateTime date);
    }
}