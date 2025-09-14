using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Data;

namespace ShoeShop.Controllers
{
    public class VoucherController : Controller
    {
        private readonly AppDbContext _context;

        public VoucherController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Apply(string voucherCode, double subTotal)
        {
            var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == voucherCode);
            if (voucher == null)
            {
                return Json(new { success = false, message = "Invalid voucher code." });
            }
            if (voucher.Number <= 0)
            {
                return Json(new { success = false, message = "This voucher is out of stock." });
            }
            if (!voucher.Status)
            {
                return Json(new { success = false, message = "This voucher is inactive." });
            }

            double discountAmount = voucher.Type ? (subTotal * (voucher.Discount / 100.0)) : (subTotal - voucher.Discount);

            if (discountAmount < 0)
            {
                discountAmount = 0; 
            }
            return Json(new { success = true, discount = discountAmount });
        }
    }
}
