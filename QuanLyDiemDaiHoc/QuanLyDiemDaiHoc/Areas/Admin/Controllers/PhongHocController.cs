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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/PhongHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhongHoc phongHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phongHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm phòng học thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(phongHoc);
        }

        // GET: Admin/PhongHoc/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var phongHoc = await _context.PhongHocs.FindAsync(id);
            if (phongHoc == null) return NotFound();

            return View(phongHoc);
        }

        // POST: Admin/PhongHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PhongHoc phongHoc)
        {
            if (id != phongHoc.MaPhong) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phongHoc);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin phòng học thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PhongHocs.Any(e => e.MaPhong == phongHoc.MaPhong)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(phongHoc);
        }

        // POST: Admin/PhongHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phongHoc = await _context.PhongHocs
                .Include(p => p.ThoiKhoaBieus) 
                .FirstOrDefaultAsync(p => p.MaPhong == id);

            if (phongHoc != null)
            {
                // Có thể kiểm tra ràng buộc khóa ngoại nếu phòng đang được sử dụng
                _context.PhongHocs.Remove(phongHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa phòng học thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

