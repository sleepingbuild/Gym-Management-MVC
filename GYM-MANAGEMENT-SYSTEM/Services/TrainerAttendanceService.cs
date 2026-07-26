using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class TrainerAttendanceService : ITrainerAttendanceService
    {
        private readonly ITrainerAttendanceRepository _repository;
        private readonly ITrainerService _trainerService;

        public TrainerAttendanceService(
            ITrainerAttendanceRepository repository,
            ITrainerService trainerService)
        {
            _repository = repository;
            _trainerService = trainerService;
        }

        public async Task<TrainerAttendanceStatusViewModel> GetStatusAsync(int trainerId)
        {
            var today = DateTime.UtcNow.Date;
            var todayRecord = await _repository.GetByTrainerAndDateAsync(trainerId, today);

            var history = (await _repository.GetByTrainerAsync(trainerId))
                .Take(14)
                .Select(a => new TrainerAttendanceHistoryItem
                {
                    Date = a.Date,
                    CheckInTime = a.CheckInTime,
                    Notes = a.Notes
                })
                .ToList();

            return new TrainerAttendanceStatusViewModel
            {
                HasCheckedInToday = todayRecord != null,
                TodayCheckInTime = todayRecord?.CheckInTime,
                RecentHistory = history
            };
        }

        public async Task CheckInAsync(int trainerId, string? notes)
        {
            var today = DateTime.UtcNow.Date;
            var existing = await _repository.GetByTrainerAndDateAsync(trainerId, today);

            if (existing != null)
            {
                throw new InvalidOperationException("Bạn đã chấm công cho hôm nay rồi.");
            }

            var attendance = new TrainerAttendance
            {
                TrainerId = trainerId,
                Date = today,
                CheckInTime = DateTime.UtcNow,
                Status = "Present",
                Notes = notes ?? string.Empty
            };

            await _repository.AddAsync(attendance);
        }

        public async Task<AdminAttendanceReportViewModel> GetDailyReportAsync(DateTime date)
        {
            var dateOnly = date.Date;
            var allTrainers = (await _trainerService.GetAllTrainersAsync()).ToList();
            var records = (await _repository.GetByDateAsync(dateOnly)).ToList();

            var rows = allTrainers.Select(t =>
            {
                var record = records.FirstOrDefault(r => r.TrainerId == t.Id);
                return new AdminAttendanceRowViewModel
                {
                    TrainerId = t.Id,
                    TrainerName = t.FullName,
                    HasCheckedIn = record != null,
                    CheckInTime = record?.CheckInTime,
                    Notes = record?.Notes ?? string.Empty
                };
            })
            .OrderBy(r => r.HasCheckedIn)
            .ThenBy(r => r.TrainerName)
            .ToList();

            return new AdminAttendanceReportViewModel
            {
                Date = dateOnly,
                TotalTrainers = allTrainers.Count,
                PresentCount = rows.Count(r => r.HasCheckedIn),
                AbsentCount = rows.Count(r => !r.HasCheckedIn),
                Rows = rows
            };
        }
    }
}