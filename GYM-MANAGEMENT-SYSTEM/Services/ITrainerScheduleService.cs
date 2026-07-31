using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface ITrainerScheduleService
    {
        Task<IEnumerable<TrainerSchedule>> GetAllSchedulesAsync();
        Task<IEnumerable<TrainerSchedule>> GetSchedulesByTrainerIdAsync(int trainerId);
        Task<IEnumerable<TrainerSchedule>> GetAvailableSlotsAsync(int trainerId, DateTime date);

       
        Task<IEnumerable<TrainerSchedule>> GetSchedulesByWeekAsync(DateOnly weekStart, DateOnly weekEnd, int? trainerId = null);

        Task<TrainerSchedule?> GetScheduleByIdAsync(int id);
        Task<TrainerSchedule> CreateScheduleAsync(ScheduleCreateViewModel model);
        Task<TrainerSchedule> UpdateScheduleAsync(ScheduleEditViewModel model);
        Task<bool> DeleteScheduleAsync(int id);
        Task<bool> ToggleScheduleStatusAsync(int id);
        Task<IEnumerable<DayOfWeek>> GetAvailableDaysForTrainerAsync(int trainerId);
        Task<bool> IsTimeSlotAvailableAsync(int trainerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludeId = null);
    }
}