using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface ITrainerAttendanceService
    {
        Task<TrainerAttendanceStatusViewModel> GetStatusAsync(int trainerId);
        Task CheckInAsync(int trainerId, string? notes);
        Task<AdminAttendanceReportViewModel> GetDailyReportAsync(DateTime date);
    }
}