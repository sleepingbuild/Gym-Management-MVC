using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Identity;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
        }

        public async Task<IdentityResult> RegisterAsync(string email, string password, string fullName, string phoneNumber)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = phoneNumber,
                FullName = fullName,
                CreatedAt = DateTime.UtcNow,
                UserProfile = new UserProfile
                {
                    UserId = email,
                    Weight = 0,
                    Height = 0,
                    Age = 0,
                    Goal = "Chưa có mục tiêu"
                }
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Kiểm tra role Member đã tồn tại chưa
                if (!await _roleManager.RoleExistsAsync("Member"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Member"));
                }
                // Kiểm tra role Admin
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                // Kiểm tra role Trainer
                if (!await _roleManager.RoleExistsAsync("Trainer"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Trainer"));
                }

                await _userManager.AddToRoleAsync(user, "Member");
            }

            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> IsEmailTakenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}