using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public TaiKhoanController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: /Admin/TaiKhoan/Index
        public async Task<IActionResult> Index()
        {
            // Kiểm tra session đăng nhập
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Lấy danh sách tài khoản từ cơ sở dữ liệu (giả định bảng tài khoản của bạn là TaiKhoans hoặc tương tự)
            var danhSachTaiKhoan = await _context.TaiKhoans.ToListAsync();
            return View(danhSachTaiKhoan);
        }

        // POST: /Admin/TaiKhoan/ToggleStatus (Khóa/Mở khóa tài khoản)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            // Đảo trạng thái hoạt động (Ví dụ: TrangThai = true là Hoạt động, false là Khóa)
            // Bạn có thể tùy chỉnh tên thuộc tính TrangThai cho khớp với Model thực tế trong CSDL
            taiKhoan.TrangThai = !taiKhoan.TrangThai;
            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã thay đổi trạng thái tài khoản {taiKhoan.TenDangNhap} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/TaiKhoan/ResetPassword (Đặt lại mật khẩu mặc định)
        // POST: /Admin/TaiKhoan/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string tenDangNhap, string matKhauMoi)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(tenDangNhap);
            if (taiKhoan == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản!";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrEmpty(matKhauMoi) || matKhauMoi.Length < 4)
            {
                TempData["Error"] = "Mật khẩu mới phải có ít nhất 4 ký tự!";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật mật khẩu mới (Nếu hệ thống có mã hóa mật khẩu như BCrypt/SHA256, bạn mã hóa ở đây)
            taiKhoan.MatKhau = matKhauMoi;

            _context.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã đổi mật khẩu thành công cho tài khoản: {tenDangNhap}!";
            return RedirectToAction(nameof(Index));
        }
    }
}
