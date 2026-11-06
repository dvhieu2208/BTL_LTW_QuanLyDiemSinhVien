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

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.MonHocs.AsQueryable();

            // Tìm kiếm theo Mã môn hoặc Tên môn học
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => m.MaMon.Contains(searchString) || m.TenMon.Contains(searchString));
            }

            var model = await query.ToListAsync();
            return View(model);
        }

        // GET: Admin/MonHoc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/MonHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonHoc monHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm môn học thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(monHoc);
        }

        // GET: Admin/MonHoc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var monHoc = await _context.MonHocs.FindAsync(id);
            if (monHoc == null) return NotFound();

            return View(monHoc);
        }

        // POST: Admin/MonHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MonHoc monHoc)
        {
            if (id != monHoc.MaMon) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monHoc);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin môn học thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MonHocs.Any(e => e.MaMon == monHoc.MaMon)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(monHoc);
        }

        // POST: Admin/MonHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var monHoc = await _context.MonHocs
                .Include(m => m.Diems) 
                .FirstOrDefaultAsync(m => m.MaMon == id);

            if (monHoc != null)
            {
                if (monHoc.Diems != null && monHoc.Diems.Any())
                {
                    TempData["Error"] = "Không thể xóa môn học này vì đã có dữ liệu điểm của sinh viên!";
                    return RedirectToAction(nameof(Index));
                }

                _context.MonHocs.Remove(monHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa môn học thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}