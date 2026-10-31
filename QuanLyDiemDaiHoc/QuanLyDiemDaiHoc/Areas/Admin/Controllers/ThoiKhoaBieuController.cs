using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

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
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.ThoiKhoaBieus 
                .Include(l => l.MaLopNavigation)
                .Include(l => l.MaMonNavigation)
                .Include(l => l.MaPhongNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.MaLop.Contains(searchString) ||
                                         l.MaMonNavigation.TenMon.Contains(searchString) ||
                                         l.MaPhong.Contains(searchString));
            }

            var model = await query.ToListAsync();
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop");
            ViewBag.MaMon = new SelectList(_context.MonHocs, "MaMon", "TenMon");
            ViewBag.MaPhong = new SelectList(_context.PhongHocs, "MaPhong", "TenPhong");
            return View();
        }

        // POST: Admin/LichHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThoiKhoaBieu lichHoc)
        {
            ModelState.Remove("MaLopNavigation");
            ModelState.Remove("MaMonNavigation");
            ModelState.Remove("MaPhongNavigation");
            bool isExist = await _context.ThoiKhoaBieus.AnyAsync(l => l.MaLop == lichHoc.MaLop && l.MaMon == lichHoc.MaMon);
            if (isExist)
            {
                ModelState.AddModelError(string.Empty, "Lịch học cho lớp này với môn học này đã tồn tại trong hệ thống!");
            }
            if (ModelState.IsValid)
            {
                _context.Add(lichHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm lịch học thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop", lichHoc.MaLop);
            ViewBag.MaMon = new SelectList(_context.MonHocs, "MaMon", "TenMon", lichHoc.MaMon);
            ViewBag.MaPhong = new SelectList(_context.PhongHocs, "MaPhong", "TenPhong", lichHoc.MaPhong);
            return View(lichHoc);
        }

        // GET: Admin/LichHoc/Edit/5
        public async Task<IActionResult> Edit(string maLop, string maMon)
        {
            if (string.IsNullOrEmpty(maLop) || string.IsNullOrEmpty(maMon)) return NotFound();

            var lichHoc = await _context.ThoiKhoaBieus.FirstOrDefaultAsync(l => l.MaLop == maLop && l.MaMon == maMon);
            if (lichHoc == null) return NotFound();

            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop", lichHoc.MaLop);
            ViewBag.MaMon = new SelectList(_context.MonHocs, "MaMon", "TenMon", lichHoc.MaMon);
            ViewBag.MaPhong = new SelectList(_context.PhongHocs, "MaPhong", "TenPhong", lichHoc.MaPhong);
            return View(lichHoc);
        }

        // POST: Admin/LichHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string maLop, string maMon, ThoiKhoaBieu lichHoc)
        {
            ModelState.Remove("MaLopNavigation");
            ModelState.Remove("MaMonNavigation");
            ModelState.Remove("MaPhongNavigation");
            if (maLop != lichHoc.MaLop || maMon != lichHoc.MaMon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichHoc);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật lịch học thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ThoiKhoaBieus.Any(e => e.MaLop == lichHoc.MaLop && e.MaMon == lichHoc.MaMon))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop", lichHoc.MaLop);
            ViewBag.MaMon = new SelectList(_context.MonHocs, "MaMon", "TenMon", lichHoc.MaMon);
            ViewBag.MaPhong = new SelectList(_context.PhongHocs, "MaPhong", "TenPhong", lichHoc.MaPhong);
            return View(lichHoc);
        }

        // POST: Admin/LichHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string maLop, string maMon)
        {
            var lichHoc = await _context.ThoiKhoaBieus.FirstOrDefaultAsync(l => l.MaLop == maLop && l.MaMon == maMon);
            if (lichHoc != null)
            {
                _context.ThoiKhoaBieus.Remove(lichHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa lịch học thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
