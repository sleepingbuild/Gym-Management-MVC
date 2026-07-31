using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GYM_MANAGEMENT_SYSTEM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<MembershipPackage> MembershipPackages { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<WorkoutProgress> WorkoutProgresses { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<ChatSummary> ChatSummaries { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<TrainerSchedule> TrainerSchedules { get; set; }
        public DbSet<TrainerAttendance> TrainerAttendances { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }

        // Face Attendance
        public DbSet<FaceProfile> FaceProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.UserProfile)
                .WithOne(up => up.ApplicationUser)
                .HasForeignKey<ApplicationUser>(u => u.UserProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<MembershipPackage>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Entity<Membership>()
                .HasOne(m => m.MembershipPackage)
                .WithMany(p => p.Memberships)
                .HasForeignKey(m => m.MembershipPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Trainer)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.Membership)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainerAttendance>()
                .HasOne(a => a.Trainer)
                .WithMany()
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mỗi trainer chỉ có tối đa 1 bản ghi chấm công / ngày
            builder.Entity<TrainerAttendance>()
                .HasIndex(a => new { a.TrainerId, a.Date })
                .IsUnique();

            // Mỗi user chỉ có 1 hồ sơ khuôn mặt (đăng ký lại sẽ update)
            builder.Entity<FaceProfile>()
                .HasOne(f => f.ApplicationUser)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FaceProfile>()
                .HasIndex(f => f.UserId)
                .IsUnique();
        }
    }
}