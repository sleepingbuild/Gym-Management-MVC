using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class MembershipPackageService : IMembershipPackageService
    {
        private readonly IMembershipPackageRepository _repository;

        public MembershipPackageService(IMembershipPackageRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MembershipPackage>> GetAllPackagesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<MembershipPackage>> GetActivePackagesAsync()
        {
            return await _repository.GetActivePackagesAsync();
        }

        public async Task<MembershipPackage?> GetPackageByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MembershipPackage> CreatePackageAsync(PackageCreateViewModel model)
        {
            // Kiểm tra tên duy nhất
            if (!await _repository.IsNameUniqueAsync(model.Name))
            {
                throw new InvalidOperationException("Tên gói tập đã tồn tại.");
            }

            var package = new MembershipPackage
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DurationDays = model.DurationDays,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            return await _repository.AddAsync(package);
        }

        public async Task<MembershipPackage> UpdatePackageAsync(PackageEditViewModel model)
        {
            var package = await _repository.GetByIdAsync(model.Id);
            if (package == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói tập.");
            }

            // Kiểm tra tên duy nhất (trừ chính nó)
            if (!await _repository.IsNameUniqueAsync(model.Name, model.Id))
            {
                throw new InvalidOperationException("Tên gói tập đã tồn tại.");
            }

            package.Name = model.Name;
            package.Description = model.Description;
            package.Price = model.Price;
            package.DurationDays = model.DurationDays;
            package.IsActive = model.IsActive;

            return await _repository.UpdateAsync(package);
        }

        public async Task<bool> DeletePackageAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> TogglePackageStatusAsync(int id)
        {
            var package = await _repository.GetByIdAsync(id);
            if (package == null)
                return false;

            package.IsActive = !package.IsActive;
            await _repository.UpdateAsync(package);
            return true;
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            return await _repository.IsNameUniqueAsync(name, excludeId);
        }

        public async Task<int> GetPackageCountAsync()
        {
            return await _repository.CountAsync();
        }
    }
}