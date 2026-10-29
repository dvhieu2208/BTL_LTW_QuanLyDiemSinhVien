using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public HomeController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            ViewBag.TongSinhVien = await _context.SinhViens.CountAsync();
            ViewBag.TongLop = await _context.Lops.CountAsync();
            ViewBag.TongMonHoc = await _context.MonHocs.CountAsync();
            ViewBag.TongKhoa = await _context.Khoas.CountAsync();

            var sinhVienNoiBat = await _context.SinhViens.Include(s => s.MaLopNavigation).Take(5).ToListAsync();

            ViewBag.SinhVienNoiBat = sinhVienNoiBat;

            return View();
        }
    }
}
