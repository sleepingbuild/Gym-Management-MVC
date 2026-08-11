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

            // 5. Seed lịch sử doanh thu 6 tháng trước — để Dashboard/biểu đồ doanh thu
            // hiển thị xu hướng thực tế thay vì chỉ có vài giao dịch gần đây.
            if (!context.Payments.Any())
            {
                var allMembers = await userManager.GetUsersInRoleAsync("Member");
                var packages = context.MembershipPackages.ToList();
                var rnd = new Random();

                if (allMembers.Any() && packages.Any())
                {
                    // Số giao dịch mỗi tháng — xu hướng tăng dần (mô phỏng phòng gym
                    // ngày càng đông khách), có dao động nhẹ cho thực tế.
                    var monthsAgoToCount = new Dictionary<int, int>
        {
            { 6, 8 },   // 6 tháng trước: 8 giao dịch
            { 5, 10 },
            { 4, 9 },
            { 3, 13 },
            { 2, 15 },
            { 1, 18 }   // Tháng trước: 18 giao dịch
        };

                    foreach (var (monthsAgo, count) in monthsAgoToCount)
                    {
                        var monthDate = DateTime.Now.AddMonths(-monthsAgo);
                        var daysInMonth = DateTime.DaysInMonth(monthDate.Year, monthDate.Month);

                        for (int i = 0; i < count; i++)
                        {
                            var member = allMembers[rnd.Next(allMembers.Count)];
                            var package = packages[rnd.Next(packages.Count)];

                            var day = rnd.Next(1, daysInMonth + 1);
                            var transactionDate = new DateTime(monthDate.Year, monthDate.Month, day,
                                rnd.Next(7, 21), rnd.Next(0, 60), 0);

                            // Membership tương ứng với giao dịch này
                            var membership = new Membership
                            {
                                UserId = member.Id,
                                MembershipPackageId = package.Id,
                                StartDate = transactionDate,
                                EndDate = transactionDate.AddDays(package.DurationDays),
                                Status = transactionDate.AddDays(package.DurationDays) < DateTime.Now ? "Expired" : "Active",
                                CreatedAt = transactionDate
                            };
                            context.Memberships.Add(membership);
                            await context.SaveChangesAsync(); // lưu ngay để lấy Membership.Id

                            // Trạng thái thanh toán: đa số Success, một ít Failed/Pending cho thực tế
                            var statusRoll = rnd.Next(100);
                            var status = statusRoll < 85 ? "Success" : statusRoll < 95 ? "Failed" : "Pending";

                            context.Payments.Add(new Payment
                            {
                                UserId = member.Id,
                                MembershipId = membership.Id,
                                Amount = package.Price,
                                Method = "VNPay",
                                Status = status,
                                TransactionId = "VNP" + rnd.Next(10000000, 99999999),
                                PaymentInfo = $"Thanh toan goi {package.Name} qua VNPay",
                                CreatedAt = transactionDate
                            });
                        }
                        await context.SaveChangesAsync();
                    }
                }
            }

            // 6. Seed thêm buổi tập đã hoàn thành (Completed) trải dài 6 tháng trước
            // — để biểu đồ "Top huấn luyện viên" trên Dashboard phản ánh đúng số buổi
            // thực tế, với Phi (Team Leader) có nhiều buổi dạy nhất.
            if (context.Bookings.Count(b => b.Status == "Completed") < 40)
            {
                var trainersForStats = context.Trainers.ToList();
                var membersForStats = await userManager.GetUsersInRoleAsync("Member");
                var rndStats = new Random();

                if (trainersForStats.Any() && membersForStats.Any())
                {
                    // Số buổi đã dạy mong muốn theo FullName — trainer không có trong
                    // danh sách này sẽ nhận mặc định 4 buổi.
                    var sessionCountByTrainer = new Dictionary<string, int>
        {
            { "Phi", 25 },
            { "Nguyễn Văn Hùng", 16 },
            { "Lê Minh Đức", 13 },
            { "Phạm Thu Hà", 11 },
            { "Đỗ Quốc Bảo", 9 },
            { "Trần Thị Mai", 7 },
            { "Hoàng Anh Tuấn", 6 },
            { "Ngô Thị Lan", 5 }
        };

                    var timeSlots = new[] { "07:00-08:00", "09:00-10:00", "11:00-12:00", "14:00-15:00", "16:00-17:00", "18:00-19:00", "19:00-20:00" };

                    foreach (var trainer in trainersForStats)
                    {
                        var sessionCount = sessionCountByTrainer.TryGetValue(trainer.FullName, out var count) ? count : 4;

                        for (int i = 0; i < sessionCount; i++)
                        {
                            var member = membersForStats[rndStats.Next(membersForStats.Count)];
                            var daysAgo = rndStats.Next(1, 180); // trong 6 tháng gần nhất
                            var sessionDate = DateTime.Now.AddDays(-daysAgo).Date;
                            var timeSlot = timeSlots[rndStats.Next(timeSlots.Length)];

                            context.Bookings.Add(new Booking
                            {
                                UserId = member.Id,
                                TrainerId = trainer.Id,
                                SessionDate = sessionDate,
                                TimeSlot = timeSlot,
                                Status = "Completed",
                                Notes = "Buổi tập đã hoàn thành (seed data)",
                                CreatedAt = sessionDate
                            });
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }

            // 7. Seed lịch tập đầy đủ trạng thái cho TẤT CẢ trainer — CHỈ tạo booking
            // rơi đúng vào giờ làm việc cố định (TrainerSchedule) thật của từng
            // trainer, vì hệ thống hiện chặn đặt lịch ngoài giờ làm việc.
            if (!context.Bookings.Any(b => b.Status == "NoShow"))
            {
                var allTrainers = context.Trainers.ToList();
                var allMembersForSchedule = await userManager.GetUsersInRoleAsync("Member");
                var rndSchedule = new Random();

                if (allTrainers.Any() && allMembersForSchedule.Any())
                {
                    var today = DateTime.Now.Date;
                    var diffFromMonday = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                    var currentWeekMonday = today.AddDays(-diffFromMonday);

                    // Cộng dồn danh sách khung giờ 1 tiếng hợp lệ (vd "07:00-08:00")
                    // nằm trong khoảng StartTime-EndTime của 1 TrainerSchedule.
                    static List<string> HourlySlotsOf(TrainerSchedule sch)
                    {
                        var slots = new List<string>();
                        var t = sch.StartTime;
                        while (t.AddHours(1) <= sch.EndTime)
                        {
                            slots.Add($"{t:HH:mm}-{t.AddHours(1):HH:mm}");
                            t = t.AddHours(1);
                        }
                        return slots;
                    }

                    // Ngày gần nhất (kể cả trùng) rơi đúng vào 1 DayOfWeek cho trước, tính từ "from".
                    static DateTime NextOrSameDayOfWeek(DateTime from, DayOfWeek targetDay)
                    {
                        int diff = ((int)targetDay - (int)from.DayOfWeek + 7) % 7;
                        return from.AddDays(diff);
                    }

                    foreach (var trainer in allTrainers)
                    {
                        var schedules = context.TrainerSchedules
                            .Where(s => s.TrainerId == trainer.Id && s.IsActive)
                            .ToList();

                        // Trainer chưa có giờ làm việc cố định -> không thể tạo booking hợp lệ
                        if (!schedules.Any())
                        {
                            continue;
                        }

                        var usedSlotsToday = new HashSet<string>();
                        var usedThisWeek = new HashSet<(DateTime date, string slot)>();

                        (TrainerSchedule sch, string slot) RandomScheduleSlot()
                        {
                            for (int attempt = 0; attempt < 10; attempt++)
                            {
                                var sch = schedules[rndSchedule.Next(schedules.Count)];
                                var hourly = HourlySlotsOf(sch);
                                if (hourly.Count == 0) continue;
                                return (sch, hourly[rndSchedule.Next(hourly.Count)]);
                            }
                            var fallbackSch = schedules[0];
                            var fallbackSlots = HourlySlotsOf(fallbackSch);
                            return (fallbackSch, fallbackSlots.FirstOrDefault() ?? $"{fallbackSch.StartTime:HH:mm}-{fallbackSch.StartTime.AddHours(1):HH:mm}");
                        }

                        // ----- Cancelled: 3 buổi, trong khoảng ±6 tuần quanh hôm nay -----
                        for (int i = 0; i < 3; i++)
                        {
                            var (sch, slot) = RandomScheduleSlot();
                            var weekOffset = rndSchedule.Next(-6, 5);
                            var date = NextOrSameDayOfWeek(currentWeekMonday.AddDays(weekOffset * 7), sch.DayOfWeek);
                            var member = allMembersForSchedule[rndSchedule.Next(allMembersForSchedule.Count)];

                            context.Bookings.Add(new Booking
                            {
                                UserId = member.Id,
                                TrainerId = trainer.Id,
                                SessionDate = date,
                                TimeSlot = slot,
                                Status = "Cancelled",
                                Notes = "Buổi tập đã huỷ (seed data)",
                                CreatedAt = date.AddDays(-1)
                            });
                        }

                        // ----- Confirmed: 2 buổi chắc chắn tuần này + 2 buổi tuần sau đó -----
                        for (int i = 0; i < 4; i++)
                        {
                            TrainerSchedule sch; string slot; DateTime date;
                            int tries = 0;
                            do
                            {
                                (sch, slot) = RandomScheduleSlot();
                                date = i < 2
                                    ? NextOrSameDayOfWeek(currentWeekMonday, sch.DayOfWeek)
                                    : NextOrSameDayOfWeek(currentWeekMonday.AddDays(rndSchedule.Next(1, 5) * 7), sch.DayOfWeek);
                                tries++;
                            } while (!usedThisWeek.Add((date, slot)) && tries < 20);

                            var member = allMembersForSchedule[rndSchedule.Next(allMembersForSchedule.Count)];
                            context.Bookings.Add(new Booking
                            {
                                UserId = member.Id,
                                TrainerId = trainer.Id,
                                SessionDate = date,
                                TimeSlot = slot,
                                Status = "Confirmed",
                                Notes = "Đã xác nhận (seed data)",
                                CreatedAt = today
                            });
                        }

                        // ----- Pending: 2 buổi chắc chắn tuần này + 2 buổi tuần sau đó -----
                        for (int i = 0; i < 4; i++)
                        {
                            TrainerSchedule sch; string slot; DateTime date;
                            int tries = 0;
                            do
                            {
                                (sch, slot) = RandomScheduleSlot();
                                date = i < 2
                                    ? NextOrSameDayOfWeek(currentWeekMonday, sch.DayOfWeek)
                                    : NextOrSameDayOfWeek(currentWeekMonday.AddDays(rndSchedule.Next(1, 5) * 7), sch.DayOfWeek);
                                tries++;
                            } while (!usedThisWeek.Add((date, slot)) && tries < 20);

                            var member = allMembersForSchedule[rndSchedule.Next(allMembersForSchedule.Count)];
                            context.Bookings.Add(new Booking
                            {
                                UserId = member.Id,
                                TrainerId = trainer.Id,
                                SessionDate = date,
                                TimeSlot = slot,
                                Status = "Pending",
                                Notes = "Đang chờ xác nhận (seed data)",
                                CreatedAt = today
                            });
                        }

                        // ----- Completed + NoShow: CHỈ hôm nay, và CHỈ nếu trainer có ca làm hôm nay -----
                        var todaySchedules = schedules.Where(s => s.DayOfWeek == today.DayOfWeek).ToList();
                        if (todaySchedules.Any())
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                var sch = todaySchedules[rndSchedule.Next(todaySchedules.Count)];
                                var hourly = HourlySlotsOf(sch);
                                if (hourly.Count == 0) continue;

                                string slot;
                                int tries = 0;
                                do { slot = hourly[rndSchedule.Next(hourly.Count)]; tries++; }
                                while ((!usedSlotsToday.Add(slot) || usedThisWeek.Contains((today, slot))) && tries < 10);

                                var member = allMembersForSchedule[rndSchedule.Next(allMembersForSchedule.Count)];
                                context.Bookings.Add(new Booking
                                {
                                    UserId = member.Id,
                                    TrainerId = trainer.Id,
                                    SessionDate = today,
                                    TimeSlot = slot,
                                    Status = "Completed",
                                    Notes = "Đã hoàn thành buổi tập hôm nay (seed data)",
                                    CreatedAt = today
                                });
                            }

                            var noShowSch = todaySchedules[rndSchedule.Next(todaySchedules.Count)];
                            var noShowHourly = HourlySlotsOf(noShowSch);
                            if (noShowHourly.Count > 0)
                            {
                                string slot;
                                int tries = 0;
                                do { slot = noShowHourly[rndSchedule.Next(noShowHourly.Count)]; tries++; }
                                while ((!usedSlotsToday.Add(slot) || usedThisWeek.Contains((today, slot))) && tries < 10);

                                var member = allMembersForSchedule[rndSchedule.Next(allMembersForSchedule.Count)];
                                context.Bookings.Add(new Booking
                                {
                                    UserId = member.Id,
                                    TrainerId = trainer.Id,
                                    SessionDate = today,
                                    TimeSlot = slot,
                                    Status = "NoShow",
                                    Notes = "[Tự động đánh dấu Không đến do quá giờ tập mà học viên không điểm danh]",
                                    CreatedAt = today
                                });
                            }
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}