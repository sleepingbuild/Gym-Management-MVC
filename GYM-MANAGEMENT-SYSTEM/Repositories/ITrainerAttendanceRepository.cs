using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface ITrainerAttendanceRepository
    {
        Task<TrainerAttendance?> GetByTrainerAndDateAsync(int trainerId, DateTime date);
        Task<IEnumerable<TrainerAttendance>> GetByTrainerAsync(int trainerId);
        Task<IEnumerable<TrainerAttendance>> GetByDateAsync(DateTime date);
        Task<TrainerAttendance> AddAsync(TrainerAttendance attendance);
    }
}