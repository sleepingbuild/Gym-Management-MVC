using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // 1. Tạo Role
            string[] roles = { "Admin", "Trainer", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Seed Membership Packages
            if (!context.MembershipPackages.Any())
            {
                context.MembershipPackages.AddRange(
                    new MembershipPackage { Name = "Gói Cơ Bản", Description = "Sử dụng phòng gym, không kèm huấn luyện viên riêng", Price = 300000, DurationDays = 30, IsActive = true, CreatedAt = DateTime.Now },
                    new MembershipPackage { Name = "Gói Tiêu Chuẩn", Description = "Sử dụng phòng gym + 4 buổi tập cùng huấn luyện viên/tháng", Price = 600000, DurationDays = 30, IsActive = true, CreatedAt = DateTime.Now },
                    new MembershipPackage { Name = "Gói VIP", Description = "Sử dụng phòng gym không giới hạn + huấn luyện viên riêng + tư vấn dinh dưỡng", Price = 1200000, DurationDays = 30, IsActive = true, CreatedAt = DateTime.Now },
                    new MembershipPackage { Name = "Gói 6 Tháng Tiết Kiệm", Description = "Gói Tiêu Chuẩn áp dụng trong 6 tháng, tiết kiệm 15%", Price = 3000000, DurationDays = 180, IsActive = true, CreatedAt = DateTime.Now }
                );
                await context.SaveChangesAsync();
            }

            // 3. Seed Trainers (kèm tài khoản đăng nhập)
            if (!context.Trainers.Any())
            {
                var trainersData = new[]
                {
                    new { FullName = "Nguyễn Văn Hùng", Email = "hung.trainer@gymsystem.com", Specialization = "Tăng cơ, Gym cơ bản", Bio = "5 năm kinh nghiệm huấn luyện thể hình", Phone = "0901234561" },
                    new { FullName = "Trần Thị Mai", Email = "mai.trainer@gymsystem.com", Specialization = "Yoga, Giảm cân", Bio = "Chuyên gia yoga và dinh dưỡng giảm cân", Phone = "0901234562" },
                    new { FullName = "Lê Minh Đức", Email = "duc.trainer@gymsystem.com", Specialization = "Powerlifting, Sức mạnh", Bio = "Cựu vận động viên powerlifting quốc gia", Phone = "0901234563" },
                    new { FullName = "Phạm Thu Hà", Email = "ha.trainer@gymsystem.com", Specialization = "Cardio, Fitness nữ", Bio = "Chuyên huấn luyện fitness cho nữ giới", Phone = "0901234564" },
                    new { FullName = "Hoàng Anh Tuấn", Email = "tuan.trainer@gymsystem.com", Specialization = "Phục hồi chấn thương, Gym trị liệu", Bio = "Chứng chỉ vật lý trị liệu thể thao", Phone = "0901234565" },
                    new { FullName = "Đỗ Quốc Bảo", Email = "bao.trainer@gymsystem.com", Specialization = "CrossFit, HIIT", Bio = "Huấn luyện viên CrossFit cấp độ 2", Phone = "0901234566" },
                    new { FullName = "Ngô Thị Lan", Email = "lan.trainer@gymsystem.com", Specialization = "Pilates, Giãn cơ", Bio = "Chuyên gia Pilates trị liệu", Phone = "0901234567" }
                };

                foreach (var t in trainersData)
                {
                    var user = new ApplicationUser
                    {
                        UserName = t.Email,
                        Email = t.Email,
                        FullName = t.FullName,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(user, "Trainer@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Trainer");

                        var trainer = new Trainer
                        {
                            UserId = user.Id,
                            FullName = t.FullName,
                            Specialization = t.Specialization,
                            Bio = t.Bio,
                            Phone = t.Phone,
                            Email = t.Email,
                            IsAvailable = true,
                            CreatedAt = DateTime.Now
                        };
                        context.Trainers.Add(trainer);
                        await context.SaveChangesAsync(); // save để lấy trainer.Id ngay

                        // Lịch làm việc cố định: Thứ 2-6, sáng 7h-11h, chiều 14h-18h
                        context.TrainerSchedules.AddRange(
                        new TrainerSchedule { TrainerId = trainer.Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(11, 0), Notes = "Ca sáng", IsActive = true, CreatedAt = DateTime.Now },
                        new TrainerSchedule { TrainerId = trainer.Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(18, 0), Notes = "Ca chiều", IsActive = true, CreatedAt = DateTime.Now },
                        new TrainerSchedule { TrainerId = trainer.Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(11, 0), Notes = "Ca sáng", IsActive = true, CreatedAt = DateTime.Now }
                        );
                    }
                }
                await context.SaveChangesAsync();
            }

            // 4. Seed Members (khách hàng) + Membership + Booking
            if (!context.Memberships.Any())
            {
                var membersData = new[]
                {
                    new { FullName = "Vũ Thành Nam", Email = "nam.member@gymsystem.com" },
                    new { FullName = "Đặng Thị Thu", Email = "thu.member@gymsystem.com" },
                    new { FullName = "Bùi Văn Long", Email = "long.member@gymsystem.com" },
                    new { FullName = "Trịnh Ngọc Anh", Email = "anh.member@gymsystem.com" },
                    new { FullName = "Lý Gia Bảo", Email = "baolg.member@gymsystem.com" },
                    new { FullName = "Cao Thị Hương", Email = "huong.member@gymsystem.com" }
                };

                var packages = context.MembershipPackages.ToList();
                var trainers = context.Trainers.ToList();
                var rnd = new Random();

                var timeSlots = new[] { "07:00-08:00", "09:00-10:00", "14:00-15:00", "16:00-17:00", "18:00-19:00" };
                var statuses = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };

                foreach (var m in membersData)
                {
                    var user = new ApplicationUser
                    {
                        UserName = m.Email,
                        Email = m.Email,
                        FullName = m.FullName,
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(user, "Member@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Member");

                        // Đăng ký gói tập ngẫu nhiên
                        var package = packages[rnd.Next(packages.Count)];
                        context.Memberships.Add(new Membership
                        {
                            UserId = user.Id,
                            MembershipPackageId = package.Id,
                            StartDate = DateTime.Now.AddDays(-rnd.Next(1, 20)),
                            EndDate = DateTime.Now.AddDays(package.DurationDays - rnd.Next(1, 20)),
                            Status = "Active",
                            CreatedAt = DateTime.Now
                        });

                        // Mỗi member đặt 2-4 booking với các trainer/ngày/giờ khác nhau
                        int bookingCount = rnd.Next(2, 5);
                        for (int i = 0; i < bookingCount; i++)
                        {
                            var trainer = trainers[rnd.Next(trainers.Count)];
                            var sessionDate = DateTime.Now.AddDays(rnd.Next(-10, 15)).Date;
                            var timeSlot = timeSlots[rnd.Next(timeSlots.Length)];
                            var status = sessionDate < DateTime.Now.Date
                                ? statuses[rnd.Next(2, 4)]   // ngày quá khứ -> Completed hoặc Cancelled
                                : statuses[rnd.Next(0, 2)];  // ngày tương lai -> Pending hoặc Confirmed

                            context.Bookings.Add(new Booking
                            {
                                UserId = user.Id,
                                TrainerId = trainer.Id,
                                SessionDate = sessionDate,
                                TimeSlot = timeSlot,
                                Status = status,
                                Notes = "Đặt lịch tự động (seed data)",
                                CreatedAt = DateTime.Now
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ Lỗi tạo member {m.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}