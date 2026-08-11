using System.Text.Json;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class FaceProfileService : IFaceProfileService
    {
        private readonly IFaceProfileRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public FaceProfileService(
            IFaceProfileRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task SaveFaceAsync(string userId, float[] descriptor)
        {
            if (descriptor == null || descriptor.Length == 0)
            {
                throw new InvalidOperationException("Không nhận được dữ liệu khuôn mặt hợp lệ. Vui lòng thử lại.");
            }

            var profile = new FaceProfile
            {
                UserId = userId,
                DescriptorJson = JsonSerializer.Serialize(descriptor)
            };

            await _repository.UpsertAsync(profile);
        }

        public async Task<bool> HasFaceProfileAsync(string userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);
            return profile != null;
        }

        public async Task<IEnumerable<KioskFaceProfileViewModel>> GetAllForKioskAsync()
        {
            var profiles = await _repository.GetAllAsync();

            return profiles.Select(p => new KioskFaceProfileViewModel
            {
                UserId = p.UserId,
                FullName = p.ApplicationUser?.FullName ?? "N/A",
                Descriptor = JsonSerializer.Deserialize<float[]>(p.DescriptorJson) ?? Array.Empty<float>()
            });
        }

        public async Task<float[]?> GetDescriptorAsync(string userId)
        {
            var profile = await _repository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<float[]>(profile.DescriptorJson);
        }

        public async Task<IEnumerable<FaceEnrollableUserViewModel>> GetEnrollableUsersAsync()
        {
            var allUsers = _userManager.Users.ToList();
            var enrolledUserIds = (await _repository.GetAllAsync())
                .Select(p => p.UserId)
                .ToHashSet();

            var result = new List<FaceEnrollableUserViewModel>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new FaceEnrollableUserViewModel
                {
                    UserId = user.Id,
                    FullName = string.IsNullOrWhiteSpace(user.FullName) ? (user.Email ?? user.Id) : user.FullName,
                    Email = user.Email ?? string.Empty,
                    Role = roles.Count > 0 ? string.Join(", ", roles) : "Member",
                    HasFaceProfile = enrolledUserIds.Contains(user.Id)
                });
            }

            return result
                .OrderBy(u => u.HasFaceProfile)
                .ThenBy(u => u.FullName)
                .ToList();
        }

        private const double MatchThreshold = 0.45;

        public async Task<string?> FindMatchingUserIdAsync(float[] descriptor)
        {
            if (descriptor == null || descriptor.Length == 0)
            {
                return null;
            }

            var profiles = await _repository.GetAllAsync();

            string? bestUserId = null;
            double bestDistance = double.MaxValue;

            foreach (var profile in profiles)
            {
                var stored = JsonSerializer.Deserialize<float[]>(profile.DescriptorJson);
                if (stored == null || stored.Length != descriptor.Length)
                {
                    continue;
                }

                var distance = EuclideanDistance(descriptor, stored);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestUserId = profile.UserId;
                }
            }

            return bestDistance < MatchThreshold ? bestUserId : null;
        }

        public async Task<bool> VerifyOwnFaceAsync(string userId, float[] descriptor)
        {
            if (descriptor == null || descriptor.Length == 0)
            {
                return false;
            }

            var profile = await _repository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                return false;
            }

            var stored = JsonSerializer.Deserialize<float[]>(profile.DescriptorJson);
            if (stored == null || stored.Length != descriptor.Length)
            {
                return false;
            }

            return EuclideanDistance(descriptor, stored) < MatchThreshold;
        }

        private static double EuclideanDistance(float[] a, float[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                var diff = a[i] - b[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }
    }
}