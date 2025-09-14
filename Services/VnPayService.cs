using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ShoeShop.Services
{
    public class VnPayService
    {
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _vnpUrl;
        private readonly string _returnUrl;

        public VnPayService(string tmnCode, string hashSecret, string vnpUrl, string returnUrl)
        {
            _tmnCode = tmnCode;
            _hashSecret = hashSecret;
            _vnpUrl = vnpUrl;
            _returnUrl = returnUrl;
        }

        public string CreatePaymentUrl(decimal amount, string orderInfo, string ipAddress)
        {
            if (ipAddress == "::1") ipAddress = "127.0.0.1";

            decimal usdToVndRate = 25000;
            decimal amountInVnd = amount * usdToVndRate;

            // Đảm bảo vnp_Amount là số nguyên dương
            int vnpAmount = (int)(Math.Round(amountInVnd * 100, 0));

            // Tạo TxnRef chỉ chứa số (10 chữ số)
            string txnRef = new Random().Next(1000000000, 1999999999).ToString();

            var vnpayData = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _tmnCode },
                { "vnp_Amount", vnpAmount.ToString() }, // Đúng định dạng
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", txnRef }, // Chỉ chứa số
                { "vnp_OrderInfo", orderInfo },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", _returnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }
            };

            var queryString = new StringBuilder();
            foreach (var item in vnpayData)
            {
                if (!string.IsNullOrEmpty(item.Value))
                    queryString.Append($"{item.Key}={HttpUtility.UrlEncode(item.Value)}&");
            }

            string rawData = queryString.ToString().TrimEnd('&');
            string secureHash = HmacSHA512(_hashSecret, rawData);
            queryString.Append($"vnp_SecureHash={secureHash}");

            return $"{_vnpUrl}?{queryString}";
        }

        private string HmacSHA512(string key, string input)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
            }
        }
    }
}
