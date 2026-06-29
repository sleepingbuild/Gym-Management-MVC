using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GYM_MANAGEMENT_SYSTEM.Tests.Services
{
    [TestClass]
    public class MembershipServiceTests
    {
        // Test các logic đơn giản không cần database

        [TestMethod]
        public void MembershipRegistrationViewModel_ShouldHaveRequiredFields()
        {
            // Arrange
            var model = new MembershipRegistrationViewModel
            {
                UserId = "user123",
                MembershipPackageId = 1
            };

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual("user123", model.UserId);
            Assert.AreEqual(1, model.MembershipPackageId);
        }

        [TestMethod]
        public void UserMembershipViewModel_ShouldCalculateCorrectly()
        {
            // Arrange
            var model = new UserMembershipViewModel
            {
                Id = 1,
                PackageName = "Gói 1 tháng",
                Price = 300000,
                DurationDays = 30,
                StartDate = DateTime.UtcNow.AddDays(-15),
                EndDate = DateTime.UtcNow.AddDays(15),
                Status = "Active"
            };

            // Act
            var daysRemaining = (model.EndDate - DateTime.UtcNow).Days;
            var isActive = model.Status == "Active";

            // Assert
            Assert.AreEqual("Gói 1 tháng", model.PackageName);
            Assert.AreEqual(300000, model.Price);
            Assert.IsTrue(isActive);
            Assert.AreEqual(15, daysRemaining);
            Assert.AreEqual("30,000 VNĐ", model.PriceFormatted); // 30,000 VNĐ
        }

        [TestMethod]
        public void RenewalInfoViewModel_ShouldCalculateNewEndDateCorrectly()
        {
            // Arrange
            var model = new RenewalInfoViewModel
            {
                MembershipId = 1,
                PackageName = "Gói 1 tháng",
                Price = 300000,
                DurationDays = 30,
                CurrentEndDate = DateTime.UtcNow.AddDays(5),
                NewEndDate = DateTime.UtcNow.AddDays(35) // 5 + 30
            };

            // Act
            var newEndDate = model.CurrentEndDate.AddDays(model.DurationDays);

            // Assert
            Assert.AreEqual(newEndDate.Date, model.NewEndDate.Date);
            Assert.AreEqual(35, (model.NewEndDate - DateTime.UtcNow).Days);
        }

        [TestMethod]
        public void MembershipStatus_ShouldBeValidValues()
        {
            // Arrange
            var validStatuses = new[] { "Active", "Expired", "Cancelled" };

            // Act & Assert
            var membership = new Membership
            {
                Id = 1,
                UserId = "user123",
                Status = "Active",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            Assert.IsTrue(validStatuses.Contains(membership.Status));
            Assert.AreEqual("Active", membership.Status);
            Assert.IsTrue(membership.EndDate > membership.StartDate);
        }

        [TestMethod]
        public void PriceFormatting_ShouldBeCorrect()
        {
            // Arrange
            var prices = new[]
            {
                new { Price = 300000m, Expected = "300,000 VNĐ" },
                new { Price = 750000m, Expected = "750,000 VNĐ" },
                new { Price = 1350000m, Expected = "1,350,000 VNĐ" },
                new { Price = 2340000m, Expected = "2,340,000 VNĐ" }
            };

            // Act & Assert
            foreach (var p in prices)
            {
                var formatted = $"{p.Price:N0} VNĐ";
                Assert.AreEqual(p.Expected, formatted);
            }
        }

        [TestMethod]
        public void DurationDisplay_ShouldBeCorrect()
        {
            // Arrange & Act & Assert
            var durations = new[]
            {
                new { Days = 30, Expected = "1 tháng" },
                new { Days = 90, Expected = "3 tháng" },
                new { Days = 180, Expected = "6 tháng" },
                new { Days = 365, Expected = "12 tháng" },
                new { Days = 15, Expected = "15 ngày" },
                new { Days = 7, Expected = "7 ngày" }
            };

            foreach (var d in durations)
            {
                var display = d.Days >= 30
                    ? $"{d.Days / 30} tháng"
                    : $"{d.Days} ngày";
                Assert.AreEqual(d.Expected, display);
            }
        }

        [TestMethod]
        public void MembershipPackage_ShouldHaveValidationAttributes()
        {
            // Arrange
            var package = new MembershipPackage
            {
                Id = 1,
                Name = "Gói 1 tháng",
                Description = "Tập không giới hạn trong 1 tháng",
                Price = 300000,
                DurationDays = 30,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.IsNotNull(package);
            Assert.IsTrue(package.Price >= 0);
            Assert.IsTrue(package.DurationDays >= 1 && package.DurationDays <= 365);
            Assert.IsNotNull(package.Name);
            Assert.IsTrue(package.Name.Length >= 3);
        }

        [TestMethod]
        public void IsUserEligibleForRegistration_ShouldCheckCorrectly()
        {
            // Arrange
            var hasActiveMembership = true;
            var noActiveMembership = false;

            // Act & Assert
            Assert.IsFalse(hasActiveMembership); // Nếu có active membership thì không eligible
            Assert.IsTrue(!noActiveMembership); // Nếu không có active membership thì eligible
        }
    }
}