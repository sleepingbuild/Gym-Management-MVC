using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IMembershipPackageService
    {
        Task<IEnumerable<MembershipPackage>> GetAllPackagesAsync();
        Task<IEnumerable<MembershipPackage>> GetActivePackagesAsync();
        Task<MembershipPackage?> GetPackageByIdAsync(int id);
        Task<MembershipPackage> CreatePackageAsync(PackageCreateViewModel model);
        Task<MembershipPackage> UpdatePackageAsync(PackageEditViewModel model);
        Task<bool> DeletePackageAsync(int id);
        Task<bool> TogglePackageStatusAsync(int id);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task<int> GetPackageCountAsync();
    }
}