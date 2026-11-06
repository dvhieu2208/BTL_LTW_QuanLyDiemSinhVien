using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DotDangKyHocPhanController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public DotDangKyHocPhanController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        private bool LaQuanTri()
        {
            return HttpContext.Session.GetInt32("PhanQuyen") == 0;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!LaQuanTri())
                return RedirectToAction("Login", "Account", new { area = "" });

            var cauHinh = await _context.CauHinhDangKyHocPhans
                .FirstOrDefaultAsync(c => c.Id == 1);

            // Chỉ dùng để giao diện hiển thị đúng trạng thái "chưa thiết lập"
            ViewBag.DaCoCauHinh = cauHinh != null;

            return View(cauHinh ?? new CauHinhDangKyHocPhan
            {
                Id = 1,
                NgayBatDau = DateTime.Now,
                NgayKetThuc = DateTime.Now.AddDays(7)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            if (!LaQuanTri())
                return RedirectToAction("Login", "Account", new { area = "" });

            var model = new CauHinhDangKyHocPhan
            {
                Id = 1,
                NgayBatDau = ngayBatDau,
                NgayKetThuc = ngayKetThuc
            };

            if (ngayBatDau == default || ngayKetThuc == default ||
                ngayKetThuc <= ngayBatDau)
            {
                ModelState.AddModelError(string.Empty,
                    "Ngày kết thúc phải sau ngày bắt đầu.");
                return View(model);
            }

            var cauHinh = await _context.CauHinhDangKyHocPhans
                .FirstOrDefaultAsync(c => c.Id == 1);

            if (cauHinh == null)
            {
                _context.CauHinhDangKyHocPhans.Add(model);
            }
            else
            {
                cauHinh.NgayBatDau = ngayBatDau;
                cauHinh.NgayKetThuc = ngayKetThuc;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã lưu thời hạn đăng ký học phần.";
            return RedirectToAction(nameof(Index));
        }
    }
}
