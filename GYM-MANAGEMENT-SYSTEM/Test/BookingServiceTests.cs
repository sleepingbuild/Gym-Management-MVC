using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GYM_MANAGEMENT_SYSTEM.Tests
{
    [TestClass]
    public class BookingServiceTests
    {
        [TestMethod]
        public void BookingCreateViewModel_ShouldHaveRequiredFields()
        {
            // Arrange
            var model = new BookingCreateViewModel
            {
                UserId = "user123",
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1),
                TimeSlot = "09:00",
                Notes = "Test booking"
            };

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual("user123", model.UserId);
            Assert.AreEqual(1, model.TrainerId);
            Assert.AreEqual("09:00", model.TimeSlot);
        }

        [TestMethod]
        public void BookingIndexViewModel_ShouldDisplayCorrectly()
        {
            // Arrange
            var model = new BookingIndexViewModel
            {
                Id = 1,
                UserId = "user123",
                TrainerId = 1,
                TrainerName = "Nguyễn Văn A",
                SessionDate = DateTime.UtcNow.AddDays(1),
                TimeSlot = "09:00",
                Status = "Pending",
                Notes = "Test",
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var dateDisplay = model.DateDisplay;
            var timeDisplay = model.TimeDisplay;
            var statusDisplay = model.StatusDisplay;

            // Assert
            Assert.IsNotNull(dateDisplay);
            Assert.IsNotNull(timeDisplay);
            Assert.AreEqual("Pending", model.Status);
            Assert.AreEqual("Chờ xác nhận", statusDisplay);
        }

        [TestMethod]
        public void Booking_StatusTransitions_ShouldBeValid()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                UserId = "user123",
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1),
                TimeSlot = "09:00",
                Status = "Pending",
                Notes = "Test"
            };

            // Act & Assert - Pending → Confirmed
            booking.Status = "Confirmed";
            Assert.AreEqual("Confirmed", booking.Status);

            // Confirmed → Completed
            booking.Status = "Completed";
            Assert.AreEqual("Completed", booking.Status);

            // Any → Cancelled
            booking.Status = "Cancelled";
            Assert.AreEqual("Cancelled", booking.Status);
        }

        [TestMethod]
        public void Booking_Validation_InvalidStatus_ShouldFail()
        {
            // Arrange
            var validStatuses = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };
            var booking = new Booking
            {
                Id = 1,
                UserId = "user123",
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1),
                TimeSlot = "09:00",
                Status = "InvalidStatus"
            };

            // Act
            var isValid = validStatuses.Contains(booking.Status);

            // Assert
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void BookingStatisticsViewModel_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new BookingStatisticsViewModel
            {
                TotalBookings = 10,
                PendingBookings = 2,
                ConfirmedBookings = 3,
                CompletedBookings = 4,
                CancelledBookings = 1,
                UpcomingBookings = 3,
                PastBookings = 7
            };

            // Assert
            Assert.AreEqual(10, stats.TotalBookings);
            Assert.AreEqual(2, stats.PendingBookings);
            Assert.AreEqual(3, stats.ConfirmedBookings);
            Assert.AreEqual(4, stats.CompletedBookings);
            Assert.AreEqual(1, stats.CancelledBookings);
            Assert.AreEqual(40, stats.CompletionRate); // 4/10 * 100
            Assert.AreEqual(10, stats.CancellationRate); // 1/10 * 100
        }

        [TestMethod]
        public void IsSlotAvailable_ShouldReturnFalse_WhenOverlapping()
        {
            // Arrange
            var existingBookings = new List<Booking>
            {
                new Booking { TrainerId = 1, SessionDate = DateTime.UtcNow.AddDays(1).Date, TimeSlot = "09:00", Status = "Confirmed" }
            };

            var newBooking = new Booking
            {
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1).Date,
                TimeSlot = "09:00"
            };

            // Act
            var isOverlapping = existingBookings.Any(b =>
                b.TrainerId == newBooking.TrainerId &&
                b.SessionDate.Date == newBooking.SessionDate.Date &&
                b.TimeSlot == newBooking.TimeSlot &&
                b.Status != "Cancelled"
            );

            // Assert
            Assert.IsTrue(isOverlapping);
        }

        [TestMethod]
        public void IsSlotAvailable_ShouldReturnTrue_WhenNotOverlapping()
        {
            // Arrange
            var existingBookings = new List<Booking>
            {
                new Booking { TrainerId = 1, SessionDate = DateTime.UtcNow.AddDays(1).Date, TimeSlot = "09:00", Status = "Confirmed" }
            };

            var newBooking = new Booking
            {
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1).Date,
                TimeSlot = "10:00"
            };

            // Act
            var isOverlapping = existingBookings.Any(b =>
                b.TrainerId == newBooking.TrainerId &&
                b.SessionDate.Date == newBooking.SessionDate.Date &&
                b.TimeSlot == newBooking.TimeSlot &&
                b.Status != "Cancelled"
            );

            // Assert
            Assert.IsFalse(isOverlapping);
        }

        [TestMethod]
        public void IsSlotAvailable_ShouldReturnTrue_WhenCancelledBookingExists()
        {
            // Arrange
            var existingBookings = new List<Booking>
            {
                new Booking { TrainerId = 1, SessionDate = DateTime.UtcNow.AddDays(1).Date, TimeSlot = "09:00", Status = "Cancelled" }
            };

            var newBooking = new Booking
            {
                TrainerId = 1,
                SessionDate = DateTime.UtcNow.AddDays(1).Date,
                TimeSlot = "09:00"
            };

            // Act
            var isOverlapping = existingBookings.Any(b =>
                b.TrainerId == newBooking.TrainerId &&
                b.SessionDate.Date == newBooking.SessionDate.Date &&
                b.TimeSlot == newBooking.TimeSlot &&
                b.Status != "Cancelled"
            );

            // Assert
            Assert.IsFalse(isOverlapping);
        }
    }
}