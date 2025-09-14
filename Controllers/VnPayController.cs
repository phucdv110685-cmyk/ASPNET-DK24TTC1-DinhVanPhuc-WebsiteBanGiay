using Microsoft.AspNetCore.Mvc;
using ShoeShop.Services;
using System.Configuration;

namespace ShoeShop.Controllers
{
    public class VnPayController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VnPayController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ActionResult Payment(decimal amount, string orderInfo)
        {
            string userIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var VnPayService = new VnPayService(
                System.Configuration.ConfigurationManager.AppSettings["VnPay:TmnCode"],
                System.Configuration.ConfigurationManager.AppSettings["VnPay:HashSecret"],
                System.Configuration.ConfigurationManager.AppSettings["VnPay:Url"],
                System.Configuration.ConfigurationManager.AppSettings["VnPay:ReturnUrl"]
            );

            string paymentUrl = VnPayService.CreatePaymentUrl(amount, orderInfo, userIpAddress);
            return Redirect(paymentUrl);
        }

        public ActionResult PaymentReturn()
        {
            var query = Request.Query;
            query.TryGetValue("vnp_ResponseCode", out var vnp_ResponseCode);
            query.TryGetValue("vnp_TxnRef", out var vnp_TxnRef);
            query.TryGetValue("vnp_Amount", out var vnp_Amount);

            if (vnp_ResponseCode == "00") 
            {
                ViewBag.Message = "Thanh toán thành công!";
                ViewBag.OrderId = vnp_TxnRef.ToString();
                ViewBag.Amount = int.Parse(vnp_Amount.ToString()) / 100;
            }
            else
            {
                ViewBag.Message = "Thanh toán thất bại!";
            }

            return View();
        }
    }
}
