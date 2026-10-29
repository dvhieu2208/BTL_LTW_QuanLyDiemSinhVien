using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Student.Controllers
{
    [Area("Student")]
    public class DiemController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public DiemController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: Student/Diem
        public async Task<IActionResult> Index()
        {
            // Lấy mã sinh viên từ Session đã lưu khi đăng nhập
            string maSv = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSv))
            {
                // Nếu chưa đăng nhập hoặc mất session, đá về trang Login
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Truy vấn điểm của sinh viên kết hợp với bảng môn học
            var danhSachDiem = await _context.Diems
                .Include(d => d.MaMonNavigation)
                .Where(d => d.MaSv == maSv)
                .ToListAsync();

            // Tính điểm trung bình (GPA) hệ 10 và tổng số tín chỉ tích lũy
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

            ViewBag.GPA = Math.Round(gpa, 2);
            ViewBag.TongTinChi = tongTinChi;

            return View(danhSachDiem);
        }
    }
}
