using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Student.Controllers
{
    [Area("Student")]
    public class HomeController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public HomeController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: Student/Home
        public async Task<IActionResult> Index()
        {
            // Lấy mã sinh viên từ Session đã lưu lúc đăng nhập
            string maSv = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSv))
            {
                // Nếu chưa đăng nhập hoặc mất session, chuyển hướng về trang Login chung
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Truy vấn thông tin sinh viên kèm theo các bảng quan hệ (Lớp, Khoa)
            var sinhVien = await _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .Include(s => s.MaKhoaNavigation)
                .FirstOrDefaultAsync(s => s.MaSv == maSv);

            // Lấy danh sách điểm của sinh viên này kèm theo thông tin môn học
            var danhSachDiem = await _context.Diems
                .Include(d => d.MaMonNavigation)
                .Where(d => d.MaSv == maSv)
                .ToListAsync();

            // Tính toán GPA (Điểm trung bình) và Tổng số tín chỉ tích lũy
            double gpa = 0;
            int tongTinChi = 0;

            if (danhSachDiem != null && danhSachDiem.Any())
            {
                var validScores = danhSachDiem.Where(d => d.Diem1.HasValue).ToList();
                if (validScores.Any())
                {
                    gpa = (double)validScores.Average(d => d.Diem1.Value);
                }
                tongTinChi = danhSachDiem.Sum(d => d.MaMonNavigation?.Dvht ?? 0);
            }

            // Truyền dữ liệu qua ViewBag sang View
            ViewBag.GPA = Math.Round(gpa, 2);
            ViewBag.TongTinChi = tongTinChi;

            return View(sinhVien);
        }
    }
}
