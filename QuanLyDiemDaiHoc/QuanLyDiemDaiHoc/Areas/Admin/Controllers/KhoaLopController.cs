using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
        [Area("Admin")]
        public class KhoaLopController : Controller
        {
            private readonly QuanLyDiemSinhVienDaiHocContext _context;

            public KhoaLopController(QuanLyDiemSinhVienDaiHocContext context)
            {
                _context = context;
            }

            // GET: Admin/Lop
            public async Task<IActionResult> Index(string searchString, string maKhoaFilter)
            {
                var query = _context.Lops.Include(l => l.MaKhoaNavigation).AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(l => l.MaLop.Contains(searchString) || l.TenLop.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(maKhoaFilter))
                {
                    query = query.Where(l => l.MaKhoa == maKhoaFilter);
                }

                var model = await query.ToListAsync();
                ViewBag.KhoaList = await _context.Khoas.ToListAsync();

                return View(model);
            }
        }
    }
