using GYM_MANAGEMENT_SYSTEM.Data;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatisticsViewModel> GetStatisticsAsync()
        {
            return await GetDetailedStatisticsAsync();
        }

        public async Task<DashboardStatisticsViewModel> GetDetailedStatisticsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            var expiringThreshold = now.AddDays(7);

            var stats = new DashboardStatisticsViewModel();

            // User Statistics
            stats.TotalUsers = await _context.Users.CountAsync();
            stats.NewUsersThisMonth = await _context.Users
                .Where(u => u.CreatedAt >= startOfMonth && u.CreatedAt < endOfMonth)
                .CountAsync();
            // ApplicationUser không có LastLogin, tạm thời set 0
            stats.ActiveUsersThisMonth = 0;

            // Membership Statistics
            stats.TotalMemberships = await _context.Memberships.CountAsync();
            stats.ActiveMemberships = await _context.Memberships
                .Where(m => m.Status == "Active")
                .CountAsync();
            stats.ExpiredMemberships = await _context.Memberships
                .Where(m => m.Status == "Expired")
                .CountAsync();
            stats.ExpiringSoonMemberships = await _context.Memberships
                .Where(m => m.Status == "Active" && m.EndDate <= expiringThreshold && m.EndDate >= now)
                .CountAsync();

            // Trainer Statistics
            stats.TotalTrainers = await _context.Trainers.CountAsync();
            stats.AvailableTrainers = await _context.Trainers
                .Where(t => t.IsAvailable)
                .CountAsync();

            // Booking Statistics
            stats.TotalBookings = await _context.Bookings.CountAsync();
            stats.BookingsThisMonth = await _context.Bookings
                .Where(b => b.CreatedAt >= startOfMonth && b.CreatedAt < endOfMonth)
                .CountAsync();
            stats.PendingBookings = await _context.Bookings
                .Where(b => b.Status == "Pending")
                .CountAsync();
            stats.CompletedBookings = await _context.Bookings
                .Where(b => b.Status == "Completed")
                .CountAsync();

            // Payment Statistics
            var successPayments = await _context.Payments
                .Where(p => p.Status == "Success")
                .ToListAsync();
            stats.TotalPayments = await _context.Payments.CountAsync();
            stats.SuccessPayments = successPayments.Count;
            stats.TotalRevenue = successPayments.Sum(p => p.Amount);
            stats.RevenueThisMonth = successPayments
                .Where(p => p.CreatedAt >= startOfMonth && p.CreatedAt < endOfMonth)
                .Sum(p => p.Amount);

            // Workout Statistics
            stats.TotalWorkoutRecords = await _context.WorkoutProgresses.CountAsync();
            stats.WorkoutRecordsThisMonth = await _context.WorkoutProgresses
                .Where(w => w.RecordedAt >= startOfMonth && w.RecordedAt < endOfMonth)
                .CountAsync();

            return stats;
        }

        public async Task<DashboardChartViewModel> GetChartDataAsync()
        {
            var chartData = new DashboardChartViewModel();

            var revenueData = await GetMonthlyRevenueAsync(6);
            chartData.RevenueData = revenueData.Select(r => new ChartDataPoint
            {
                Label = r.Month,
                Value = r.Revenue
            }).ToList();

            var membershipData = await GetMonthlyMembershipAsync(6);
            chartData.MembershipData = membershipData.Select(m => new ChartDataPoint
            {
                Label = m.Month,
                Count = m.NewMemberships
            }).ToList();

            var bookingData = await _context.Bookings
                .Where(b => b.CreatedAt >= DateTime.UtcNow.AddMonths(-6))
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new ChartDataPoint
                {
                    Label = $"{g.Key.Month}/{g.Key.Year}",
                    Count = g.Count()
                })
                .OrderBy(d => d.Label)
                .ToListAsync();

            chartData.BookingData = bookingData;
            return chartData;
        }

        public async Task<List<MonthlyRevenueViewModel>> GetMonthlyRevenueAsync(int months = 6)
        {
            var result = new List<MonthlyRevenueViewModel>();
            var now = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var startDate = new DateTime(month.Year, month.Month, 1);
                var endDate = startDate.AddMonths(1);

                var revenue = await _context.Payments
                    .Where(p => p.Status == "Success" && p.CreatedAt >= startDate && p.CreatedAt < endDate)
                    .SumAsync(p => p.Amount);
                var count = await _context.Payments
                    .Where(p => p.Status == "Success" && p.CreatedAt >= startDate && p.CreatedAt < endDate)
                    .CountAsync();

                result.Add(new MonthlyRevenueViewModel
                {
                    Month = $"{month:MM/yyyy}",
                    Revenue = revenue,
                    Count = count
                });
            }
            return result;
        }

        public async Task<List<MonthlyMembershipViewModel>> GetMonthlyMembershipAsync(int months = 6)
        {
            var result = new List<MonthlyMembershipViewModel>();
            var now = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                var startDate = new DateTime(month.Year, month.Month, 1);
                var endDate = startDate.AddMonths(1);

                var newMemberships = await _context.Memberships
                    .Where(m => m.CreatedAt >= startDate && m.CreatedAt < endDate)
                    .CountAsync();
                var renewals = await _context.Memberships
                    .Where(m => m.CreatedAt >= startDate && m.CreatedAt < endDate && m.Status == "Active")
                    .CountAsync();
                var cancellations = await _context.Memberships
                    .Where(m => m.CreatedAt >= startDate && m.CreatedAt < endDate && m.Status == "Cancelled")
                    .CountAsync();

                result.Add(new MonthlyMembershipViewModel
                {
                    Month = $"{month:MM/yyyy}",
                    NewMemberships = newMemberships,
                    Renewals = renewals,
                    Cancellations = cancellations
                });
            }
            return result;
        }

        public async Task<List<ChartDataPoint>> GetTopTrainersAsync(int count = 5)
        {
            return await _context.Bookings
                .Where(b => b.Status == "Completed")
                .GroupBy(b => b.TrainerId)
                .Select(g => new ChartDataPoint
                {
                    Label = _context.Trainers.Where(t => t.Id == g.Key).Select(t => t.FullName).FirstOrDefault() ?? "Unknown",
                    Count = g.Count()
                })
                .OrderByDescending(d => d.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<ChartDataPoint>> GetMembershipPackageDistributionAsync()
        {
            return await _context.Memberships
                .Where(m => m.Status == "Active")
                .GroupBy(m => m.MembershipPackageId)
                .Select(g => new ChartDataPoint
                {
                    Label = _context.MembershipPackages.Where(p => p.Id == g.Key).Select(p => p.Name).FirstOrDefault() ?? "Unknown",
                    Count = g.Count()
                })
                .OrderByDescending(d => d.Count)
                .ToListAsync();
        }

        public async Task<List<ChartDataPoint>> GetBookingStatusDistributionAsync()
        {
            return await _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<ChartDataPoint>> GetPaymentStatusDistributionAsync()
        {
            return await _context.Payments
                .GroupBy(p => p.Status)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<ChartDataPoint>> GetRevenueByMethodAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == "Success")
                .GroupBy(p => p.Method)
                .Select(g => new ChartDataPoint
                {
                    Label = g.Key,
                    Value = g.Sum(p => p.Amount)
                })
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetDailyRevenueAsync(int days = 30)
        {
            var result = new Dictionary<string, decimal>();
            var now = DateTime.UtcNow.Date;

            for (int i = days - 1; i >= 0; i--)
            {
                var date = now.AddDays(-i);
                var startDate = date;
                var endDate = date.AddDays(1);

                var revenue = await _context.Payments
                    .Where(p => p.Status == "Success" && p.CreatedAt >= startDate && p.CreatedAt < endDate)
                    .SumAsync(p => p.Amount);

                result[date.ToString("dd/MM")] = revenue;
            }
            return result;
        }
    }
}