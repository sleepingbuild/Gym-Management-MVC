using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class TrainerScheduleService : ITrainerScheduleService
    {
        private readonly ITrainerScheduleRepository _repository;
        private readonly ITrainerRepository _trainerRepository;

        public TrainerScheduleService(
            ITrainerScheduleRepository repository,
            ITrainerRepository trainerRepository)
        {
            _repository = repository;
            _trainerRepository = trainerRepository;
        }

        public async Task<IEnumerable<TrainerSchedule>> GetAllSchedulesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<TrainerSchedule>> GetSchedulesByTrainerIdAsync(int trainerId)
        {
            return await _repository.GetByTrainerIdAsync(trainerId);
        }

        public async Task<IEnumerable<TrainerSchedule>> GetAvailableSlotsAsync(int trainerId, DateTime date)
        {
            return await _repository.GetAvailableSlotsAsync(trainerId, date);
        }

        public async Task<IEnumerable<TrainerSchedule>> GetSchedulesByWeekAsync(DateOnly weekStart, DateOnly weekEnd, int? trainerId = null)
        {
            return await _repository.GetByDateRangeAsync(weekStart, weekEnd, trainerId);
        }

        public async Task<TrainerSchedule?> GetScheduleByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<TrainerSchedule> CreateScheduleAsync(ScheduleCreateViewModel model)
        {
            // Kiểm tra trainer tồn tại
            var trainer = await _trainerRepository.GetByIdAsync(model.TrainerId);
            if (trainer == null)
            {
                throw new KeyNotFoundException("Không tìm thấy huấn luyện viên.");
            }

    
            if (await _repository.HasWorkDateScheduleConflictAsync(
                model.TrainerId,
                model.WorkDate,
                model.StartTime,
                model.EndTime))
            {
                throw new InvalidOperationException("Khung giờ này đã có lịch. Vui lòng chọn khung giờ khác.");
            }

            // Kiểm tra thời gian hợp lệ
            if (model.StartTime >= model.EndTime)
            {
                throw new InvalidOperationException("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");
            }

            var schedule = new TrainerSchedule
            {
                TrainerId = model.TrainerId,
                WorkDate = model.WorkDate,
                DayOfWeek = model.WorkDate.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Notes = model.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.AddAsync(schedule);
        }

        public async Task<TrainerSchedule> UpdateScheduleAsync(ScheduleEditViewModel model)
        {
            var schedule = await _repository.GetByIdAsync(model.Id);
            if (schedule == null)
            {
                throw new KeyNotFoundException("Không tìm thấy lịch.");
            }

            // Kiểm tra trùng lịch theo ngày cụ thể (trừ chính nó)
            if (await _repository.HasWorkDateScheduleConflictAsync(
                schedule.TrainerId,
                model.WorkDate,
                model.StartTime,
                model.EndTime,
                model.Id))
            {
                throw new InvalidOperationException("Khung giờ này đã có lịch. Vui lòng chọn khung giờ khác.");
            }

            // Kiểm tra thời gian hợp lệ
            if (model.StartTime >= model.EndTime)
            {
                throw new InvalidOperationException("Thời gian bắt đầu phải nhỏ hơn thời gian kết thúc.");
            }

            schedule.WorkDate = model.WorkDate;
            schedule.DayOfWeek = model.WorkDate.DayOfWeek;
            schedule.StartTime = model.StartTime;
            schedule.EndTime = model.EndTime;
            schedule.Notes = model.Notes;
            schedule.IsActive = model.IsActive;

            return await _repository.UpdateAsync(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> ToggleScheduleStatusAsync(int id)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null)
                return false;

            schedule.IsActive = !schedule.IsActive;
            await _repository.UpdateAsync(schedule);
            return true;
        }

        public async Task<IEnumerable<DayOfWeek>> GetAvailableDaysForTrainerAsync(int trainerId)
        {
            var schedules = await _repository.GetByTrainerIdAsync(trainerId);
            return schedules.Select(s => s.DayOfWeek).Distinct().OrderBy(d => d);
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int trainerId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? excludeId = null)
        {
            return await _repository.IsSlotAvailableAsync(trainerId, dayOfWeek, startTime, endTime, excludeId);
        }
    }
}