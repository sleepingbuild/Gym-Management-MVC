using GYM_MANAGEMENT_SYSTEM.Models;

namespace GYM_MANAGEMENT_SYSTEM.Repositories
{
    public interface IFaceProfileRepository
    {
        Task<FaceProfile?> GetByUserIdAsync(string userId);
        Task<IEnumerable<FaceProfile>> GetAllAsync();
        Task<FaceProfile> UpsertAsync(FaceProfile profile);
    }
}