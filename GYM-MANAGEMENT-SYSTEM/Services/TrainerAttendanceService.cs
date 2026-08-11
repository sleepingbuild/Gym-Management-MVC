using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    // LƯU Ý MÚI GIỜ: cố tình dùng DateTime.Now (giờ máy chủ) thay vì DateTime.UtcNow
    // cho toàn bộ CheckInTime/CheckOutTime/"today" trong file này. Ca làm việc
    // (Trainer.ShiftStartTime/EndTime) do Admin nhập là giờ VN theo đồng hồ treo
    // tường, nên giờ điểm danh phải cùng "múi giờ" mới so sánh đúng (tính đi muộn/
    // về sớm) và hiển thị đúng trong bảng chấm công. Nếu deploy lên server đặt ở
    // múi giờ khác VN, cần đổi lại cách tính cho phù hợp.
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
            var today = DateTime.Now.Date;
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

        public async Task CheckInAsync(int trainerId, string? notes, string method = "Manual")
        {
            var today = DateTime.Now.Date;
            var existing = await _repository.GetByTrainerAndDateAsync(trainerId, today);

            if (existing != null)
            {
                throw new InvalidOperationException("Bạn đã chấm công cho hôm nay rồi.");
            }

            var attendance = new TrainerAttendance
            {
                TrainerId = trainerId,
                Date = today,
                CheckInTime = DateTime.Now,
                Status = "Present",
                Method = method,
                Notes = notes ?? string.Empty
            };

            await _repository.AddAsync(attendance);
        }

        public async Task<bool> CheckOutAsync(int trainerId)
        {
            var today = DateTime.Now.Date;
            var record = await _repository.GetByTrainerAndDateAsync(trainerId, today);

            if (record == null)
            {
                throw new InvalidOperationException("Bạn chưa điểm danh vào ca hôm nay, không thể điểm danh tan ca.");
            }

            if (record.CheckOutTime != null)
            {
                throw new InvalidOperationException("Bạn đã điểm danh tan ca hôm nay rồi.");
            }

            record.CheckOutTime = DateTime.Now;
            await _repository.UpdateAsync(record);
            return true;
        }

        public async Task<GYM_MANAGEMENT_SYSTEM.Models.TrainerAttendance?> GetTodayRecordAsync(int trainerId)
        {
            var today = DateTime.Now.Date;
            return await _repository.GetByTrainerAndDateAsync(trainerId, today);
        }

        public async Task<AdminAttendanceReportViewModel> GetDailyReportAsync(DateTime date)
        {
            var dateOnly = date.Date;
            var allTrainers = (await _trainerService.GetAllTrainersAsync()).ToList();
            var records = (await _repository.GetByDateAsync(dateOnly)).ToList();

            var rows = allTrainers.Select(t =>
            {
                var record = records.FirstOrDefault(r => r.TrainerId == t.Id);

                int? lateMinutes = null;
                int? earlyMinutes = null;

                if (record != null && t.ShiftStartTime.HasValue)
                {
                    var checkInTimeOfDay = TimeOnly.FromDateTime(record.CheckInTime);
                    var diff = (int)(checkInTimeOfDay.ToTimeSpan() - t.ShiftStartTime.Value.ToTimeSpan()).TotalMinutes;
                    if (diff > 0)
                    {
                        lateMinutes = diff;
                    }
                }

                if (record?.CheckOutTime != null && t.ShiftEndTime.HasValue)
                {
                    var checkOutTimeOfDay = TimeOnly.FromDateTime(record.CheckOutTime.Value);
                    var diff = (int)(t.ShiftEndTime.Value.ToTimeSpan() - checkOutTimeOfDay.ToTimeSpan()).TotalMinutes;
                    if (diff > 0)
                    {
                        earlyMinutes = diff;
                    }
                }

                return new AdminAttendanceRowViewModel
                {
                    TrainerId = t.Id,
                    TrainerName = t.FullName,
                    HasCheckedIn = record != null,
                    CheckInTime = record?.CheckInTime,
                    CheckOutTime = record?.CheckOutTime,
                    Notes = record?.Notes ?? string.Empty,
                    ShiftStartTime = t.ShiftStartTime,
                    ShiftEndTime = t.ShiftEndTime,
                    LateMinutes = lateMinutes,
                    EarlyLeaveMinutes = earlyMinutes
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