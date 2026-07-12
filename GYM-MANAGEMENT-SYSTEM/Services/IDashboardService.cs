using GYM_MANAGEMENT_SYSTEM.ViewModels;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatisticsViewModel> GetStatisticsAsync();
        Task<DashboardChartViewModel> GetChartDataAsync();
        Task<List<MonthlyRevenueViewModel>> GetMonthlyRevenueAsync(int months = 6);
        Task<List<MonthlyMembershipViewModel>> GetMonthlyMembershipAsync(int months = 6);
        Task<List<ChartDataPoint>> GetTopTrainersAsync(int count = 5);
        Task<List<ChartDataPoint>> GetMembershipPackageDistributionAsync();
    }
}