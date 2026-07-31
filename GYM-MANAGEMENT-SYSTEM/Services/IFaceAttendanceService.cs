using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IFaceAttendanceService
    {
        Task<FaceCheckInResultViewModel> CheckInAsync(string userId);
    }
}