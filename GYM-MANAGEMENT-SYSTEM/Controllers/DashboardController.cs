using GYM_MANAGEMENT_SYSTEM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: /Dashboard
        public async Task<IActionResult> Index()
        {
            var stats = await _dashboardService.GetStatisticsAsync();
            var chartData = await _dashboardService.GetChartDataAsync();
            var topTrainers = await _dashboardService.GetTopTrainersAsync(5);
            var packageDistribution = await _dashboardService.GetMembershipPackageDistributionAsync();

            ViewBag.ChartData = chartData;
            ViewBag.TopTrainers = topTrainers;
            ViewBag.PackageDistribution = packageDistribution;

            return View(stats);
        }

        // GET: /Dashboard/Revenue
        public async Task<IActionResult> Revenue()
        {
            var revenueData = await _dashboardService.GetMonthlyRevenueAsync(12);
            return Json(revenueData);
        }

        // GET: /Dashboard/Membership
        public async Task<IActionResult> Membership()
        {
            var membershipData = await _dashboardService.GetMonthlyMembershipAsync(12);
            return Json(membershipData);
        }

        // GET: /Dashboard/Trainers
        public async Task<IActionResult> Trainers()
        {
            var topTrainers = await _dashboardService.GetTopTrainersAsync(5);
            return Json(topTrainers);
        }

        // GET: /Dashboard/PackageDistribution
        public async Task<IActionResult> PackageDistribution()
        {
            var distribution = await _dashboardService.GetMembershipPackageDistributionAsync();
            return Json(distribution);
        }
    }
}