using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [HttpGet]
            public async Task<IActionResult> Index(string searchString, string maKhoaFilter)
            {
            var query = _context.Lops
            .Include(l => l.MaKhoaNavigation)
            .Include(l => l.SinhViens)       
            .AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(l => l.MaLop.Contains(searchString) || l.TenLop.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(maKhoaFilter))
            {
                query = query.Where(l => l.MaKhoa == maKhoaFilter);
            }
            ViewBag.KhoaList = await _context.Khoas.ToListAsync();
            ViewBag.SelectedKhoa = maKhoaFilter;

            var model = await query.ToListAsync();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MaKhoa = new SelectList(await _context.Khoas.ToListAsync(), "MaKhoa", "TenKhoa");
            return View();
        }
        // POST: /Admin/Lop/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLop,TenLop,MaKhoa,KhoaHoc,SiSo")] Lop lop)
        {
            bool isExist = await _context.Lops.AnyAsync(l => l.MaLop == lop.MaLop);
            if (isExist)
            {
                ModelState.AddModelError("MaLop", "Mã lớp này đã tồn tại trong hệ thống!");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(lop);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Thêm lớp học thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Bẫy lỗi nếu phát sinh vấn đề kết nối hoặc ràng buộc CSDL
                    ModelState.AddModelError("", "Đã có lỗi xảy ra khi lưu vào cơ sở dữ liệu: " + ex.Message);
                }
            }

            // Sửa lại thành _context.Khoas thay vì MaKhoaNavigation
            ViewBag.MaKhoa = new SelectList(await _context.Khoas.ToListAsync(), "MaKhoa", "TenKhoa", lop.MaKhoa);
            return View(lop);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var lop = await _context.Lops.FindAsync(id);
            if (lop == null) return NotFound();

            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", lop.MaKhoa);
            return View(lop);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Lop lop)
        {
            if (id != lop.MaLop) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lop);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin lớp học thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Lops.Any(e => e.MaLop == lop.MaLop)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", lop.MaKhoa);
            return View(lop);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lop = await _context.Lops
        .Include(l => l.SinhViens)
        .FirstOrDefaultAsync(l => l.MaLop == id);

            if (lop != null)
            {
                if (lop.SinhViens != null && lop.SinhViens.Any())
                {
                    TempData["Error"] = "Không thể xóa lớp này vì vẫn còn sinh viên thuộc biên chế lớp!";
                    return RedirectToAction(nameof(Index)); // Phải trỏ về Index của Lop
                }

                _context.Lops.Remove(lop);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa lớp học thành công!";
            }

            // Đảm bảo trả về đúng trang Index của quản lý Lớp
            return RedirectToAction(nameof(Index));
        }
    }
    }
