using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;
        public AccountController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            int? phanQuyen = HttpContext.Session.GetInt32("PhanQuyen");

            if (phanQuyen == 0)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (phanQuyen == 1)
            {
                return RedirectToAction("Index", "Home", new { area = "Student" });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
        {
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }
            var taiKhoan = await _context.TaiKhoans.Include(tk => tk.MaSvNavigation) .FirstOrDefaultAsync(tk => tk.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || taiKhoan.MatKhau != matKhau)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác.";
                return View();
            }

            if (taiKhoan.TrangThai == false)
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa.";
                return View();
            }
            HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);
            HttpContext.Session.SetInt32("PhanQuyen", taiKhoan.PhanQuyen);

            if (taiKhoan.PhanQuyen == 1 && taiKhoan.MaSvNavigation != null)
            {
                HttpContext.Session.SetString("MaSV", taiKhoan.MaSvNavigation.MaSv);
                HttpContext.Session.SetString("TenSV", taiKhoan.MaSvNavigation.TenSv);
            }

            if (taiKhoan.PhanQuyen == 0)
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else if (taiKhoan.PhanQuyen == 1)
            {
                return RedirectToAction("Index", "Home", new { area = "Student" });
            }

            ViewBag.Error = "Quyền truy cập không hợp lệ.";
            return View();
        }
        [HttpPost]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
