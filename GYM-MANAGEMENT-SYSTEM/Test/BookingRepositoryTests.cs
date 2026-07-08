using GYM_MANAGEMENT_SYSTEM.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GYM_MANAGEMENT_SYSTEM.Tests
{
    [TestClass]
    public class BookingRepositoryTests
    {
        [TestMethod]
        public void Booking_ShouldHaveValidProperties()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                UserId = "user123",
                TrainerId = 2,
                SessionDate = DateTime.UtcNow.AddDays(2),
                TimeSlot = "14:00",
                Status = "Pending",
                Notes = "First session",
                CreatedAt = DateTime.UtcNow
            };

            // Assert
            Assert.AreEqual(1, booking.Id);
            Assert.AreEqual("user123", booking.UserId);
            Assert.AreEqual(2, booking.TrainerId);
            Assert.AreEqual("14:00", booking.TimeSlot);
            Assert.AreEqual("Pending", booking.Status);
            Assert.AreEqual("First session", booking.Notes);
            Assert.IsNotNull(booking.CreatedAt);
        }

        [TestMethod]
        public void Booking_DefaultStatus_ShouldBePending()
        {
            // Arrange
            var booking = new Booking
            {
                UserId = "user123",
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1),
                TimeSlot = "09:00",
                Status = "Pending"
            };

            // Assert
            Assert.AreEqual("Pending", booking.Status);
        }

        [TestMethod]
        public void Booking_DefaultCreatedAt_ShouldBeSet()
        {
            // Arrange
            var booking = new Booking
            {
                UserId = "user123",
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1),
                TimeSlot = "09:00",
                Status = "Pending"
            };

            // Act
            booking.CreatedAt = DateTime.UtcNow;

            // Assert
            Assert.IsNotNull(booking.CreatedAt);
            Assert.IsTrue(booking.CreatedAt <= DateTime.UtcNow);
        }
    }
}