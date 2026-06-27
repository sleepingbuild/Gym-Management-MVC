using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        }
    }
}