using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoeShop.Data;
using ShoeShop.Models;

namespace ShoeShop.Areas.Admin.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Area("Admin")]
    public class VoucherController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VoucherController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {

            if (_context.Vouchers != null)
            {
                ViewBag.Voucher = await _context.Vouchers.ToListAsync();
                return View();
            }
            return Problem("Entity set 'AppDbContext.Voucher'  is null.");
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Voucher post)
        {
            var existingVoucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == post.Code);
            if (existingVoucher != null)
            {
                ModelState.AddModelError("Code", "Voucher code already exists.");
                return View(post);
            }

            _context.Add(post);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Voucher");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound();
            }
            return View(voucher);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher == null)
            {
                return NotFound(new { success = false, message = "Voucher not found" });
            }
            bool isVoucherUsed = await _context.Orders.AnyAsync(o => o.VoucherId == id);
            if (isVoucherUsed)
            {
                return Json(new { success = false, message = "This voucher has been used in an order and cannot be deleted." });
            }
            _context.Vouchers.Remove(voucher);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Voucher deleted successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm] Voucher post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            _context.Update(post);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Voucher updated successfully!";
            return RedirectToAction("Index", "Voucher");
        }

        [HttpPost]
        public async Task<IActionResult> GetVoucher(string query)
        {
            var draw = int.Parse(Request.Form["draw"].FirstOrDefault());
            var skip = int.Parse(Request.Form["start"].FirstOrDefault());
            var pageSize = int.Parse(Request.Form["length"].FirstOrDefault());
            var sortColumnIndex = int.Parse(Request.Form["order[0][column]"].FirstOrDefault());
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"].FirstOrDefault();

            var vouchers = _context.Vouchers.AsQueryable();
            switch (sortColumn?.ToLower())
            {
                case "id":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Id) : vouchers.OrderByDescending(o => o.Id);
                    break;
                case "title":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Title) : vouchers.OrderByDescending(o => o.Title);
                    break;
                case "description":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Description) : vouchers.OrderByDescending(o => o.Description);
                    break;
                case "code":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Code) : vouchers.OrderByDescending(o => o.Code);
                    break;
                case "status":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Status) : vouchers.OrderByDescending(o => o.Status);
                    break;
                case "type":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Type) : vouchers.OrderByDescending(o => o.Type);
                    break;
                case "discount":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Discount) : vouchers.OrderByDescending(o => o.Discount);
                    break;
                case "number":
                    vouchers = sortColumnDirection.ToLower() == "asc" ? vouchers.OrderBy(o => o.Number) : vouchers.OrderByDescending(o => o.Number);
                    break;
                default:
                    vouchers = vouchers.OrderBy(o => o.Id);
                    break;
            }

            if (!string.IsNullOrEmpty(query))
            {
                vouchers = vouchers.Where(m => m.Title.Contains(query) || m.Description.Contains(query) || m.Code.Contains(query));
            }
            var recordsTotal = await vouchers.CountAsync();
            var data = await vouchers
                .Skip(skip)
                .Take(pageSize)
                .Select(v => new
                {
                    v.Id,
                    v.Title,
                    v.Description,
                    v.Code,
                    v.Status,
                    v.Type,
                    v.Discount,
                    v.Number
                })
                .ToListAsync();

            var jsonData = new
            {
                draw = draw,
                recordsFiltered = recordsTotal,
                recordsTotal = recordsTotal, 
                data = data
            };

            return Ok(jsonData);
        }
    }
}
