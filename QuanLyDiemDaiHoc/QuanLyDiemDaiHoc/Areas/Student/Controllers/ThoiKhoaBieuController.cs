using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Student.Controllers
{
    [Area("Student")]
    public class ThoiKhoaBieuController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public ThoiKhoaBieuController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: Student/ThoiKhoaBieu
        public async Task<IActionResult> Index()
        {
            // Lấy mã sinh viên từ Session
            string maSv = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSv))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Tìm sinh viên để biết đang học lớp nào (MaLop)
            var sinhVien = await _context.SinhViens
                .Include(s => s.MaLopNavigation)
                .FirstOrDefaultAsync(s => s.MaSv == maSv);

            if (sinhVien == null || string.IsNullOrEmpty(sinhVien.MaLop))
            {
                ViewBag.ThongBao = "Không tìm thấy thông tin lớp học của sinh viên.";
                return View(new List<ThoiKhoaBieu>());
            }

            // Lấy thời khóa biểu theo đúng mã lớp của sinh viên đó
            var tkbList = await _context.ThoiKhoaBieus
                .Include(t => t.MaLopNavigation)
                .Include(t => t.MaMonNavigation)
                .Include(t => t.MaPhongNavigation)
                .Where(t => t.MaLop == sinhVien.MaLop)
                .ToListAsync();

            ViewBag.TenLop = sinhVien.MaLopNavigation?.TenLop ?? sinhVien.MaLop;

            return View(tkbList);
        }
    }
}
