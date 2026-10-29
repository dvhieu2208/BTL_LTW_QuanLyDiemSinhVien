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
    }
}
