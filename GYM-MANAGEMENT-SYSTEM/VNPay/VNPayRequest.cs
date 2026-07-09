using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace GYM_MANAGEMENT_SYSTEM.VNPay
{
    public class VNPayRequest
    {
        private SortedList<string, string> _parameters = new();
        private readonly VNPayConfig _config;

        public VNPayRequest(VNPayConfig config)
        {
            _config = config;
        }

        public void AddParameter(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _parameters[key] = value;
            }
        }

        public string CreatePaymentUrl(string ipAddress)
        {
            // Add default parameters
            AddParameter("vnp_Version", _config.Version);
            AddParameter("vnp_Command", _config.Command);
            AddParameter("vnp_TmnCode", _config.TmnCode);
            AddParameter("vnp_CurrCode", _config.CurrCode);
            AddParameter("vnp_Locale", _config.Locale);
            AddParameter("vnp_ReturnUrl", _config.ReturnUrl);
            AddParameter("vnp_IpAddr", ipAddress);
            AddParameter("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            AddParameter("vnp_OrderInfo", $"Thanh toán gói tập Gym Management");

            // Build URL
            var queryString = GetQueryString();
            var secureHash = GetSecureHash();

            return $"{_config.BaseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public string GetQueryString()
        {
            var query = new StringBuilder();
            foreach (var param in _parameters)
            {
                if (query.Length > 0)
                {
                    query.Append('&');
                }
                query.Append($"{WebUtility.UrlEncode(param.Key)}={WebUtility.UrlEncode(param.Value)}");
            }
            return query.ToString();
        }

        public string GetSecureHash()
        {
            var data = GetQueryString();
            var hash = new StringBuilder();

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_config.HashSecret + data));
            foreach (var b in bytes)
            {
                hash.Append(b.ToString("x2"));
            }

            return hash.ToString();
        }

        public bool ValidateSignature(string inputHash, string secureHash)
        {
            return string.Equals(inputHash, secureHash, StringComparison.OrdinalIgnoreCase);
        }

        public Dictionary<string, string> ParseResponse(string queryString)
        {
            var result = new Dictionary<string, string>();
            var query = queryString.Split('&');
            foreach (var param in query)
            {
                var parts = param.Split('=');
                if (parts.Length == 2)
                {
                    result[parts[0]] = WebUtility.UrlDecode(parts[1]);
                }
            }
            return result;
        }

        public bool VerifySignature(string queryString, string hashSecret)
        {
            var query = queryString.Split('&')
                .Where(p => !p.StartsWith("vnp_SecureHash"))
                .ToArray();

            var data = string.Join("&", query.OrderBy(p => p));
            var hash = new StringBuilder();

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashSecret + data));
            foreach (var b in bytes)
            {
                hash.Append(b.ToString("x2"));
            }

            return true;
        }
    }
}