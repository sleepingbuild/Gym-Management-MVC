using GYM_MANAGEMENT_SYSTEM.Models;
using GYM_MANAGEMENT_SYSTEM.Services;
using GYM_MANAGEMENT_SYSTEM.ViewModels;
using GYM_MANAGEMENT_SYSTEM.VNPay;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;

namespace GYM_MANAGEMENT_SYSTEM.Controllers
{
	[Authorize]
	public class PaymentController : Controller
	{
		private readonly IPaymentService _paymentService;
		private readonly IMembershipService _membershipService;
		private readonly VNPayConfig _vnpayConfig;

		public PaymentController(
			IPaymentService paymentService,
			IMembershipService membershipService,
			IOptions<VNPayConfig> vnpayConfig)
		{
			_paymentService = paymentService;
			_membershipService = membershipService;
			_vnpayConfig = vnpayConfig.Value;
		}

		// GET: /Payment
		public async Task<IActionResult> Index()
		{
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
						 ?? User.Identity?.Name;

			if (string.IsNullOrEmpty(userId))
			{
				return RedirectToAction("Login", "Account");
			}

			var payments = await _paymentService.GetUserPaymentsAsync(userId);
			var viewModels = payments.Select(p => new PaymentIndexViewModel
			{
				Id = p.Id,
				UserId = p.UserId,
				MembershipId = p.MembershipId,
				Amount = p.Amount,
				Method = p.Method,
				Status = p.Status,
				TransactionId = p.TransactionId,
				PaymentInfo = p.PaymentInfo,
				CreatedAt = p.CreatedAt
			}).ToList();

			return View(viewModels);
		}

		// GET: /Payment/Create
		public async Task<IActionResult> Create(int membershipId)
		{
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
						 ?? User.Identity?.Name;

			if (string.IsNullOrEmpty(userId))
			{
				return RedirectToAction("Login", "Account");
			}

			var membership = await _membershipService.GetByIdAsync(membershipId);
			if (membership == null)
			{
				TempData["ErrorMessage"] = "Không tìm thấy gói tập.";
				return RedirectToAction("Index", "Membership");
			}

			var viewModel = new PaymentCreateViewModel
			{
				UserId = userId,
				MembershipId = membershipId,
				Amount = membership.MembershipPackage?.Price ?? 0,
				Method = "VNPay",
				PaymentInfo = $"Thanh toán gói tập #{membershipId}"
			};

			return View(viewModel);
		}

		// POST: /Payment/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(PaymentCreateViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				// Lưu payment
				var payment = await _paymentService.CreatePaymentAsync(model);

				// Chuyển sang VNPay
				var ipAddress = GetIpAddress();
				var paymentUrl = await _paymentService.CreateVNPayPaymentUrl(model.MembershipId, ipAddress);

				return Redirect(paymentUrl);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(model);
			}
		}

		// GET: /Payment/VNPayReturn
		public async Task<IActionResult> VNPayReturn()
		{
			var queryString = Request.QueryString.ToString();
			var response = new Dictionary<string, string>();


			if (!string.IsNullOrEmpty(queryString))
			{
				var query = queryString.TrimStart('?').Split('&');
				foreach (var param in query)
				{
					var parts = param.Split('=');
					if (parts.Length == 2)
					{
						response[parts[0]] = WebUtility.UrlDecode(parts[1]);
					}
				}
			}


			var result = await _paymentService.ProcessVNPayReturn(response);

			if (result)
			{
				TempData["SuccessMessage"] = "Thanh toán thành công!";
			}
			else
			{
				TempData["ErrorMessage"] = "Thanh toán thất bại. Vui lòng thử lại.";
			}

			return RedirectToAction(nameof(Index));
		}

		// GET: /Payment/History
		public async Task<IActionResult> History()
		{
			var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
						 ?? User.Identity?.Name;

			if (string.IsNullOrEmpty(userId))
			{
				return RedirectToAction("Login", "Account");
			}

			var payments = await _paymentService.GetUserPaymentsAsync(userId);
			var viewModels = payments.Select(p => new PaymentIndexViewModel
			{
				Id = p.Id,
				UserId = p.UserId,
				MembershipId = p.MembershipId,
				Amount = p.Amount,
				Method = p.Method,
				Status = p.Status,
				TransactionId = p.TransactionId,
				PaymentInfo = p.PaymentInfo,
				CreatedAt = p.CreatedAt
			}).ToList();

			// Thống kê
			var totalSuccess = payments.Where(p => p.Status == "Success").Sum(p => p.Amount);
			ViewBag.TotalSuccess = totalSuccess;
			ViewBag.TotalCount = payments.Count();
			ViewBag.SuccessCount = payments.Count(p => p.Status == "Success");
			ViewBag.FailedCount = payments.Count(p => p.Status == "Failed");
			ViewBag.PendingCount = payments.Count(p => p.Status == "Pending");

			return View(viewModels);
		}

		// GET: /Payment/Details/5
		public async Task<IActionResult> Details(int id)
		{
			var payment = await _paymentService.GetPaymentByIdAsync(id);
			if (payment == null)
			{
				return NotFound();
			}

			var viewModel = new PaymentIndexViewModel
			{
				Id = payment.Id,
				UserId = payment.UserId,
				MembershipId = payment.MembershipId,
				Amount = payment.Amount,
				Method = payment.Method,
				Status = payment.Status,
				TransactionId = payment.TransactionId,
				PaymentInfo = payment.PaymentInfo,
				CreatedAt = payment.CreatedAt
			};

			return View(viewModel);
		}

		private string GetIpAddress()
		{
			var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
			if (string.IsNullOrEmpty(ipAddress))
			{
				ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
			}
			return ipAddress ?? "127.0.0.1";
		}

        // GET: /Payment/History
        public async Task<IActionResult> History(PaymentHistoryFilterViewModel filter)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }


            IEnumerable<Payment> payments;

            if (!string.IsNullOrEmpty(filter.Status) && filter.Status != "Tất cả")
            {
                payments = await _paymentService.GetPaymentHistoryByStatusAsync(userId, filter.Status);
            }
            else if (!string.IsNullOrEmpty(filter.SearchTerm) || filter.FromDate.HasValue || filter.ToDate.HasValue)
            {
                payments = await _paymentService.SearchPaymentsAsync(userId, filter.SearchTerm, filter.FromDate, filter.ToDate);
            }
            else
            {
                payments = await _paymentService.GetPaymentHistoryAsync(userId);
            }

            var viewModels = payments.Select(p => new PaymentIndexViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                MembershipId = p.MembershipId,
                Amount = p.Amount,
                Method = p.Method,
                Status = p.Status,
                TransactionId = p.TransactionId,
                PaymentInfo = p.PaymentInfo,
                CreatedAt = p.CreatedAt
            }).ToList();

            var stats = await _paymentService.GetPaymentStatisticsAsync(userId);
            ViewBag.Statistics = stats;

            return View(viewModels);
        }

        // GET: /Payment/Statistics
        public async Task<IActionResult> Statistics()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var stats = await _paymentService.GetPaymentStatisticsAsync(userId);
            return View(stats);
        }

        // GET: /Payment/ExportCSV
        public async Task<IActionResult> ExportCSV(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var payments = await _paymentService.GetPaymentHistoryAsync(userId, fromDate, toDate);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Mã GD,Số tiền,Phương thức,Trạng thái,Mã GD VNPay,Ngày,Thông tin");

            foreach (var p in payments)
            {
                csv.AppendLine($"{p.Id},{p.Amount:N0},{p.Method},{p.Status},{p.TransactionId},{p.CreatedAt:dd/MM/yyyy HH:mm},{p.PaymentInfo}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"payment_history_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}