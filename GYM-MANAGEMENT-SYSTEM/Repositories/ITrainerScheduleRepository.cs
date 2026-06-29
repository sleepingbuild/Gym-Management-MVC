using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface ITrainerScheduleRepository
    {
        Task<IEnumerable<TrainerSchedule>> GetAllAsync();
        Task<IEnumerable<TrainerSchedule>> GetByTrainerIdAsync(int trainerId);
        Task<IEnumerable<TrainerSchedule>> GetAvailableSlotsAsync(int trainerId, DateTime date);
        Task<TrainerSchedule?> GetByIdAsync(int id);
        Task<TrainerSchedule> AddAsync(TrainerSchedule schedule);
        Task<TrainerSchedule> UpdateAsync(TrainerSchedule schedule);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsSlotAvailableAsync(int trainerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludeId = null);
        Task<bool> HasScheduleConflictAsync(int trainerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludeId = null);
    }
}