using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Student.Controllers
{
    [Area("Student")]
    public class HoSoController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public HoSoController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: Student/HoSo
        public async Task<IActionResult> Index()
        {
            // Lấy mã sinh viên từ Session đã lưu lúc đăng nhập
            string maSv = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSv))
            {
                // Nếu chưa đăng nhập, chuyển hướng về trang Login chung
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Truy vấn thông tin chi tiết sinh viên kết hợp với các bảng liên quan (Lớp, Khoa, Quê quán, Hệ đào tạo, Dân tộc, Chức vụ)
            var sinhVien = await _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .Include(s => s.MaKhoaNavigation)
                .Include(s => s.MaQueNavigation)
                .Include(s => s.MaHeDtNavigation)
                .Include(s => s.MaDanTocNavigation)
                .Include(s => s.MaChucVuNavigation)
                .FirstOrDefaultAsync(s => s.MaSv == maSv);

            if (sinhVien == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin hồ sơ sinh viên.";
                return View(new SinhVien());
            }

            return View(sinhVien);
        }
        // POST: Student/HoSo/DoiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(string matKhauHienTai, string matKhauMoi, string xacNhanMatKhau)
        {
            string maSv = HttpContext.Session.GetString("MaSV");
            if (string.IsNullOrEmpty(maSv))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            if (matKhauMoi != xacNhanMatKhau)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không khớp!";
                return RedirectToAction(nameof(Index));
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.MaSv == maSv || t.TenDangNhap == maSv);

            if (taiKhoan == null || taiKhoan.MatKhau != matKhauHienTai)
            {
                TempData["Error"] = "Mật khẩu hiện tại không chính xác!";
                return RedirectToAction(nameof(Index));
            }

            // Cập nhật mật khẩu mới vào bảng TaiKhoan
            taiKhoan.MatKhau = matKhauMoi;
            _context.TaiKhoans.Update(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
