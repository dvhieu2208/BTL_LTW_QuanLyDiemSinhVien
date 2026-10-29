using Microsoft.AspNetCore.Mvc;
using QuanLyDiemDaiHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThoiKhoaBieuController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public ThoiKhoaBieuController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: Admin/ThoiKhoaBieu
        public async Task<IActionResult> Index()
        {
            // Sử dụng các thuộc tính điều hướng chuẩn đã scaffold: MaLopNavigation, MaMonNavigation, MaPhongNavigation
            var model = await _context.ThoiKhoaBieus
                .Include(t => t.MaLopNavigation)
                .Include(t => t.MaMonNavigation)
                .Include(t => t.MaPhongNavigation)
                .ToListAsync();

            return View(model);
        }
    }
}
