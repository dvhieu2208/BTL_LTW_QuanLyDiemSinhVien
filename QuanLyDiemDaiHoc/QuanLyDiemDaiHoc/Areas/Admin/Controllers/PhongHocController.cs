using Microsoft.AspNetCore.Mvc;
using QuanLyDiemDaiHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PhongHocController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public PhongHocController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: Admin/PhongHoc
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.PhongHocs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.MaPhong.Contains(searchString) || p.TenPhong.Contains(searchString));
            }

            var model = await query.ToListAsync();
            return View(model);
        }
    }
}
