using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PackageController : Controller
    {

        private readonly IMembershipPackageService _packageService;
        private readonly IMembershipService _membershipService;

        public PackageController(IMembershipPackageService packageService, IMembershipService membershipService)
        {
            _packageService = packageService;
            _membershipService = membershipService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            var packages = await _packageService.GetAllPackagesAsync();
            var viewModels = new List<PackageIndexViewModel>();

            foreach (var p in packages)
            {
                var actionLabel = string.IsNullOrEmpty(userId)
                    ? "Đăng ký"
                    : await _membershipService.GetPackageActionLabelAsync(userId, p.Id);

                viewModels.Add(new PackageIndexViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    DurationDays = p.DurationDays,
                    MaxSessionsPerWeek = p.MaxSessionsPerWeek,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    ActionLabel = actionLabel
                });
            }

            return View(viewModels);
        }

        // GET: /Package/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            var viewModel = new PackageIndexViewModel
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                DurationDays = package.DurationDays,
                MaxSessionsPerWeek = package.MaxSessionsPerWeek,
                IsActive = package.IsActive,
                CreatedAt = package.CreatedAt
            };

            return View(viewModel);
        }

        // GET: /Package/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Package/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PackageCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _packageService.CreatePackageAsync(model);
                TempData["SuccessMessage"] = "Gói tập đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }
        }

        // GET: /Package/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
            {
                return NotFound();
            }

            var viewModel = new PackageEditViewModel
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                DurationDays = package.DurationDays,
                MaxSessionsPerWeek = package.MaxSessionsPerWeek,
                IsActive = package.IsActive
            };

            return View(viewModel);
        }

        // POST: /Package/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PackageEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _packageService.UpdatePackageAsync(model);
                TempData["SuccessMessage"] = "Gói tập đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }
        }

        // POST: /Package/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _packageService.DeletePackageAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Gói tập đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa gói tập này.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Package/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _packageService.TogglePackageStatusAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Trạng thái gói tập đã được cập nhật!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể cập nhật trạng thái gói tập.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}