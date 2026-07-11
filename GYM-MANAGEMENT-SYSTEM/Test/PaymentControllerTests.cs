using GYM_MANAGEMENT_SYSTEM.Controllers;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using GYM_MANAGEMENT_SYSTEM.VNPay;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;

namespace GYM_MANAGEMENT_SYSTEM.Tests
{
    [TestClass]
    public class PaymentControllerTests
    {
        private Mock<IPaymentService> _mockPaymentService = null!;
        private Mock<IMembershipService> _mockMembershipService = null!;
        private IOptions<VNPayConfig> _vnpayConfig = null!;
        private PaymentController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockPaymentService = new Mock<IPaymentService>();
            _mockMembershipService = new Mock<IMembershipService>();
            _vnpayConfig = Options.Create(new VNPayConfig
            {
                TmnCode = "TEST",
                HashSecret = "TEST_SECRET",
                BaseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
            });

            _controller = new PaymentController(
                _mockPaymentService.Object,
                _mockMembershipService.Object,
                _vnpayConfig);

            // Setup user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user123"),
                new Claim(ClaimTypes.Name, "user123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task Index_WithAuthenticatedUser_ShouldReturnView()
        {
            // Arrange
            var payments = new List<Payment>();
            _mockPaymentService.Setup(s => s.GetUserPaymentsAsync("user123"))
                .ReturnsAsync(payments);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_WithValidMembership_ShouldReturnView()
        {
            // Arrange
            var membershipId = 1;
            var membership = new Membership
            {
                Id = membershipId,
                UserId = "user123",
                MembershipPackage = new MembershipPackage { Price = 300000 }
            };

            _mockMembershipService.Setup(s => s.GetByIdAsync(membershipId))
                .ReturnsAsync(membership);

            // Act
            var result = await _controller.Create(membershipId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Create_WithInvalidMembership_ShouldRedirectWithError()
        {
            // Arrange
            var membershipId = 999;
            _mockMembershipService.Setup(s => s.GetByIdAsync(membershipId))
                .ReturnsAsync((Membership?)null);

            // Act
            var result = await _controller.Create(membershipId);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("Index", redirectResult.ActionName);
            Assert.AreEqual("Membership", redirectResult.ControllerName);
        }

        [TestMethod]
        public async Task History_ShouldReturnView()
        {
            // Arrange
            var payments = new List<Payment>();
            _mockPaymentService.Setup(s => s.GetPaymentHistoryAsync("user123", null, null))
                .ReturnsAsync(payments);
            _mockPaymentService.Setup(s => s.GetPaymentStatisticsAsync("user123"))
                .ReturnsAsync(new PaymentStatisticsViewModel());

            var filter = new PaymentHistoryFilterViewModel();

            // Act
            var result = await _controller.History(filter);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Statistics_ShouldReturnView()
        {
            // Arrange
            var stats = new PaymentStatisticsViewModel();
            _mockPaymentService.Setup(s => s.GetPaymentStatisticsAsync("user123"))
                .ReturnsAsync(stats);

            // Act
            var result = await _controller.Statistics();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Details_WithValidId_ShouldReturnView()
        {
            // Arrange
            var paymentId = 1;
            var payment = new Payment
            {
                Id = paymentId,
                UserId = "user123",
                Amount = 300000,
                Status = "Success"
            };

            _mockPaymentService.Setup(s => s.GetPaymentByIdAsync(paymentId))
                .ReturnsAsync(payment);

            // Act
            var result = await _controller.Details(paymentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public async Task Details_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var paymentId = 999;
            _mockPaymentService.Setup(s => s.GetPaymentByIdAsync(paymentId))
                .ReturnsAsync((Payment?)null);

            // Act
            var result = await _controller.Details(paymentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}