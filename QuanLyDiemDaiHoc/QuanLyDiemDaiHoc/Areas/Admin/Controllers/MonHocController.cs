using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyDiemDaiHoc.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MonHocController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public MonHocController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: Admin/MonHoc
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.MonHocs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(mh => mh.MaMon.Contains(searchString) || mh.TenMon.Contains(searchString));
            }

            var model = await query.ToListAsync();
            return View(model);
        }
    }
}