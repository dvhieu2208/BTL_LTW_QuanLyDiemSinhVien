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

        // GET: Admin/TaiKhoan
        public async Task<IActionResult> Index()
        {
            var model = await _context.TaiKhoans
                .Include(t => t.MaSvNavigation)
                .ToListAsync();

            return View(model);
        }
    }
}
