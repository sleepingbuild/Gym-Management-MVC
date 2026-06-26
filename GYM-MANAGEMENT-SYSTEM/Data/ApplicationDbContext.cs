using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GYM_MANAGEMENT_SYSTEM.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<FAQ> FAQs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình mối quan hệ 1-1 giữa ApplicationUser và UserProfile
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.UserProfile)
                .WithOne(up => up.ApplicationUser)
                .HasForeignKey<ApplicationUser>(u => u.UserProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình decimal precision
            builder.Entity<MembershipPackage>()
                .Property(m => m.Price)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);
        }
    }
}