using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task<IdentityResult> RegisterAsync(string email, string password, string fullName, string phoneNumber);
        Task LogoutAsync();
        Task<bool> IsEmailTakenAsync(string email);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
    }
}