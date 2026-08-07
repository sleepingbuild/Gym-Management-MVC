using GYM_MANAGEMENT_SYSTEM.Repositories;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using GYM_MANAGEMENT_SYSTEM.VNPay;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GYM_MANAGEMENT_SYSTEM.Tests
{
    [TestClass]
    public class PaymentServiceTests
    {
        private Mock<IPaymentRepository> _mockPaymentRepo;
        private Mock<IMembershipRepository> _mockMembershipRepo;
        private Mock<IMembershipPackageRepository> _mockPackageRepo;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IMembershipRenewalService> _mockRenewalService;
        private IOptions<VNPayConfig> _vnpayConfig;
        private PaymentService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockPaymentRepo = new Mock<IPaymentRepository>();
            _mockMembershipRepo = new Mock<IMembershipRepository>();
            _mockPackageRepo = new Mock<IMembershipPackageRepository>();
            _mockRenewalService = new Mock<IMembershipRenewalService>();

            // UserManager<T> không phải interface nên phải mock qua IUserStore<T>
            // rồi truyền vào constructor của Mock<UserManager<T>> (9 tham số theo
            // đúng chữ ký gốc). Các test hiện tại không gọi tới _userManager.Users
            // nên phần lớn tham số truyền null là an toàn.
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _vnpayConfig = Options.Create(new VNPayConfig
            {
                TmnCode = "TEST",
                HashSecret = "TEST_SECRET",
                BaseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
                Version = "2.1.0",
                Command = "pay",
                CurrCode = "VND",
                Locale = "vn",
                ReturnUrl = "https://localhost:5225/Payment/VNPayReturn"
            });
            _service = new PaymentService(
                _mockPaymentRepo.Object,
                _mockMembershipRepo.Object,
                _mockPackageRepo.Object,
                _mockRenewalService.Object,
                _mockUserManager.Object,
                _vnpayConfig);
        }

        // ===== CreatePaymentAsync Tests =====
        [TestMethod]
        public async Task CreatePaymentAsync_ValidData_ShouldSucceed()
        {
            // Arrange
            var userId = "user123";
            var membershipId = 1;
            var model = new PaymentCreateViewModel
            {
                UserId = userId,
                MembershipId = membershipId,
                Amount = 300000,
                Method = "VNPay",
                PaymentInfo = "Test payment"
            };

            var membership = new Membership
            {
                Id = membershipId,
                UserId = userId,
                Status = "Pending"
            };

            _mockMembershipRepo.Setup(r => r.GetByIdAsync(membershipId))
                .ReturnsAsync(membership);
            _mockPaymentRepo.Setup(r => r.AddAsync(It.IsAny<Payment>()))
                .ReturnsAsync(new Payment
                {
                    Id = 1,
                    UserId = userId,
                    MembershipId = membershipId,
                    Amount = 300000,
                    Status = "Pending"
                });

            // Act
            var result = await _service.CreatePaymentAsync(model);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.UserId);
            Assert.AreEqual(membershipId, result.MembershipId);
            Assert.AreEqual("Pending", result.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task CreatePaymentAsync_InvalidMembership_ShouldThrowException()
        {
            // Arrange
            var model = new PaymentCreateViewModel
            {
                UserId = "user123",
                MembershipId = 999,
                Amount = 300000,
                Method = "VNPay"
            };

            _mockMembershipRepo.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Membership?)null);

            // Act
            await _service.CreatePaymentAsync(model);
        }

        // ===== UpdatePaymentStatusAsync Tests =====
        [TestMethod]
        public async Task UpdatePaymentStatusAsync_ValidId_ShouldSucceed()
        {
            // Arrange
            var paymentId = 1;
            var payment = new Payment
            {
                Id = paymentId,
                UserId = "user123",
                Status = "Pending",
                Amount = 300000
            };

            _mockPaymentRepo.Setup(r => r.GetByIdAsync(paymentId))
                .ReturnsAsync(payment);
            _mockPaymentRepo.Setup(r => r.UpdateAsync(It.IsAny<Payment>()))
                .ReturnsAsync(payment);

            // Act
            var result = await _service.UpdatePaymentStatusAsync(paymentId, "Success", "TX12345");

            // Assert
            Assert.AreEqual("Success", result.Status);
            Assert.AreEqual("TX12345", result.TransactionId);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task UpdatePaymentStatusAsync_InvalidId_ShouldThrowException()
        {
            // Arrange
            _mockPaymentRepo.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Payment?)null);

            // Act
            await _service.UpdatePaymentStatusAsync(999, "Success");
        }

        // ===== GetPaymentHistoryAsync Tests =====
        [TestMethod]
        public async Task GetPaymentHistoryAsync_ShouldReturnFilteredByDate()
        {
            // Arrange
            var userId = "user123";
            var payments = new List<Payment>
            {
                new Payment { Id = 1, UserId = userId, CreatedAt = DateTime.UtcNow.AddDays(-5), Status = "Success" },
                new Payment { Id = 2, UserId = userId, CreatedAt = DateTime.UtcNow.AddDays(-10), Status = "Success" },
                new Payment { Id = 3, UserId = userId, CreatedAt = DateTime.UtcNow.AddDays(-20), Status = "Pending" }
            };

            _mockPaymentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(payments);

            var fromDate = DateTime.UtcNow.AddDays(-15);
            var toDate = DateTime.UtcNow.AddDays(-1);

            // Act
            var result = await _service.GetPaymentHistoryAsync(userId, fromDate, toDate);

            // Assert
            Assert.AreEqual(2, result.Count()); // Chỉ có 2 payment trong khoảng
        }

        [TestMethod]
        public async Task GetPaymentHistoryAsync_NoFilter_ShouldReturnAll()
        {
            // Arrange
            var userId = "user123";
            var payments = new List<Payment>
            {
                new Payment { Id = 1, UserId = userId, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Payment { Id = 2, UserId = userId, CreatedAt = DateTime.UtcNow.AddDays(-10) }
            };

            _mockPaymentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(payments);

            // Act
            var result = await _service.GetPaymentHistoryAsync(userId);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        // ===== GetPaymentHistoryByStatusAsync Tests =====
        [TestMethod]
        public async Task GetPaymentHistoryByStatusAsync_ShouldFilterCorrectly()
        {
            // Arrange
            var userId = "user123";
            var payments = new List<Payment>
            {
                new Payment { Id = 1, UserId = userId, Status = "Success" },
                new Payment { Id = 2, UserId = userId, Status = "Success" },
                new Payment { Id = 3, UserId = userId, Status = "Pending" }
            };

            _mockPaymentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(payments);

            // Act
            var result = await _service.GetPaymentHistoryByStatusAsync(userId, "Success");

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(p => p.Status == "Success"));
        }

        // ===== SearchPaymentsAsync Tests =====
        [TestMethod]
        public async Task SearchPaymentsAsync_ByTransactionId_ShouldReturnMatches()
        {
            // Arrange
            var userId = "user123";
            var payments = new List<Payment>
            {
                new Payment { Id = 1, UserId = userId, TransactionId = "TX12345", Method = "VNPay" },
                new Payment { Id = 2, UserId = userId, TransactionId = "TX67890", Method = "VNPay" }
            };

            _mockPaymentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(payments);

            // Act
            var result = await _service.SearchPaymentsAsync(userId, "TX12345");

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("TX12345", result.First().TransactionId);
        }

        [TestMethod]
        public async Task SearchPaymentsAsync_ByPaymentInfo_ShouldReturnMatches()
        {
            // Arrange
            var userId = "user123";
            var payments = new List<Payment>
            {
                new Payment { Id = 1, UserId = userId, PaymentInfo = "Gói 1 tháng", Method = "VNPay" },
                new Payment { Id = 2, UserId = userId, PaymentInfo = "Gói 3 tháng", Method = "VNPay" }
            };

            _mockPaymentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(payments);

            // Act
            var result = await _service.SearchPaymentsAsync(userId, "3 tháng");

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Gói 3 tháng", result.First().PaymentInfo);
        }

        // ===== GetPaymentStatisticsAsync Tests =====
        [TestMethod]
        public async Task GetPaymentStatisticsAsync_ShouldCalculateCorrectly()
        {
            // Arrange
            var userId = "user123";
            var payments = new List<Payment>
            {
                new Payment { Id = 1, UserId = userId, Status = "Success", Amount = 300000, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Payment { Id = 2, UserId = userId, Status = "Success", Amount = 750000, CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new Payment { Id = 3, UserId = userId, Status = "Pending", Amount = 300000, CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Payment { Id = 4, UserId = userId, Status = "Failed", Amount = 300000, CreatedAt = DateTime.UtcNow.AddDays(-7) }
            };

            _mockPaymentRepo.Setup(r => r.GetByUserIdAsync(userId))
                .ReturnsAsync(payments);

            // Act
            var result = await _service.GetPaymentStatisticsAsync(userId);

            // Assert
            Assert.AreEqual(4, result.TotalPayments);
            Assert.AreEqual(1050000, result.TotalSuccessAmount);
            Assert.AreEqual(2, result.SuccessCount);
            Assert.AreEqual(1, result.PendingCount);
            Assert.AreEqual(1, result.FailedCount);
            Assert.AreEqual(50, result.SuccessRate); // 2/4 * 100
        }
    }
}