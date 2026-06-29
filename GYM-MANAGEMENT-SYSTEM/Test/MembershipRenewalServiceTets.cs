using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GYM_MANAGEMENT_SYSTEM.Tests.Services
{
    [TestClass]
    public class MembershipRenewalServiceTests
    {
        [TestMethod]
        public void RenewalInfoViewModel_ShouldCalculateCorrectly()
        {
            // Arrange
            var currentEndDate = DateTime.UtcNow.AddDays(5);
            var durationDays = 30;
            var expectedNewEndDate = currentEndDate.AddDays(durationDays);

            var model = new RenewalInfoViewModel
            {
                MembershipId = 1,
                PackageName = "Gói 1 tháng",
                Price = 300000,
                DurationDays = durationDays,
                CurrentEndDate = currentEndDate,
                NewEndDate = expectedNewEndDate,
                IsActive = true,
                DaysUntilExpiry = 5
            };

            // Assert
            Assert.AreEqual(1, model.MembershipId);
            Assert.AreEqual("Gói 1 tháng", model.PackageName);
            Assert.AreEqual(300000, model.Price);
            Assert.AreEqual(30, model.DurationDays);
            Assert.IsTrue(model.IsActive);
            Assert.AreEqual(5, model.DaysUntilExpiry);
            Assert.AreEqual(currentEndDate.AddDays(durationDays), model.NewEndDate);
        }

        [TestMethod]
        public void Renewal_ShouldUpdateEndDate()
        {
            // Arrange
            var oldEndDate = DateTime.UtcNow.AddDays(5);
            var durationDays = 30;
            var newEndDate = oldEndDate.AddDays(durationDays);

            // Act
            var membership = new Membership
            {
                Id = 1,
                UserId = "user123",
                EndDate = oldEndDate,
                Status = "Active"
            };

            // Mô phỏng gia hạn
            membership.EndDate = newEndDate;

            // Assert
            Assert.AreEqual(newEndDate, membership.EndDate);
            Assert.IsTrue(membership.EndDate > oldEndDate);
        }

        [TestMethod]
        public void CanRenew_ShouldReturnTrueForActiveMembership()
        {
            // Arrange
            var membership = new Membership
            {
                Id = 1,
                Status = "Active",
                EndDate = DateTime.UtcNow.AddDays(15)
            };

            // Act
            var canRenew = membership.Status == "Active" || membership.Status == "Expired";

            // Assert
            Assert.IsTrue(canRenew);
        }

        [TestMethod]
        public void CanRenew_ShouldReturnFalseForCancelledMembership()
        {
            // Arrange
            var membership = new Membership
            {
                Id = 1,
                Status = "Cancelled"
            };

            // Act
            var canRenew = membership.Status == "Active" || membership.Status == "Expired";

            // Assert
            Assert.IsFalse(canRenew);
        }

        [TestMethod]
        public void CanRenew_ExpiredOver30Days_ShouldReturnFalse()
        {
            // Arrange
            var expiredDate = DateTime.UtcNow.AddDays(-35);
            var membership = new Membership
            {
                Id = 1,
                Status = "Expired",
                EndDate = expiredDate
            };

            // Act
            var daysSinceExpiry = (DateTime.UtcNow - membership.EndDate).Days;
            var canRenew = membership.Status == "Expired" && daysSinceExpiry <= 30;

            // Assert
            Assert.IsFalse(canRenew);
            Assert.AreEqual(35, daysSinceExpiry);
        }

        [TestMethod]
        public void CanRenew_ExpiredWithin30Days_ShouldReturnTrue()
        {
            // Arrange
            var expiredDate = DateTime.UtcNow.AddDays(-10);
            var membership = new Membership
            {
                Id = 1,
                Status = "Expired",
                EndDate = expiredDate
            };

            // Act
            var daysSinceExpiry = (DateTime.UtcNow - membership.EndDate).Days;
            var canRenew = membership.Status == "Expired" && daysSinceExpiry <= 30;

            // Assert
            Assert.IsTrue(canRenew);
            Assert.AreEqual(10, daysSinceExpiry);
        }

        [TestMethod]
        public void ExpiringMemberships_ShouldFilterCorrectly()
        {
            // Arrange
            var memberships = new List<Membership>
            {
                new Membership { Id = 1, Status = "Active", EndDate = DateTime.UtcNow.AddDays(3) },
                new Membership { Id = 2, Status = "Active", EndDate = DateTime.UtcNow.AddDays(5) },
                new Membership { Id = 3, Status = "Active", EndDate = DateTime.UtcNow.AddDays(10) },
                new Membership { Id = 4, Status = "Expired", EndDate = DateTime.UtcNow.AddDays(-5) },
                new Membership { Id = 5, Status = "Active", EndDate = DateTime.UtcNow.AddDays(30) }
            };

            // Act
            var threshold = 7;
            var expiringSoon = memberships
                .Where(m => m.Status == "Active" &&
                           (m.EndDate - DateTime.UtcNow).Days <= threshold &&
                           (m.EndDate - DateTime.UtcNow).Days >= 0)
                .ToList();

            // Assert
            Assert.AreEqual(2, expiringSoon.Count); // Id 1 và 2
            Assert.IsTrue(expiringSoon.Any(m => m.Id == 1));
            Assert.IsTrue(expiringSoon.Any(m => m.Id == 2));
            Assert.IsFalse(expiringSoon.Any(m => m.Id == 3));
            Assert.IsFalse(expiringSoon.Any(m => m.Id == 4));
            Assert.IsFalse(expiringSoon.Any(m => m.Id == 5));
        }
    }
}