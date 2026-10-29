using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SinhVienController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public SinhVienController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: /Admin/SinhVien/Index
        public async Task<IActionResult> Index(string searchString, string maLop)
        {
            // Kiểm tra đăng nhập
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Lấy danh sách lớp phục vụ cho bộ lọc tìm kiếm
            ViewBag.MaLop = new SelectList(await _context.Lops.ToListAsync(), "MaLop", "TenLop", maLop);
            ViewBag.CurrentFilter = searchString;
            ViewBag.SelectedLop = maLop;

            var danhSachSV = _context.SinhViens
                .Include(s => s.MaKhoaNavigation)
                .Include(s => s.MaLopNavigation)
                .Include(s => s.MaQueNavigation)
                .Include(s => s.MaHeDtNavigation)
                .AsQueryable();

            // Tìm kiếm theo tên hoặc mã sinh viên
            if (!string.IsNullOrEmpty(searchString))
            {
                danhSachSV = danhSachSV.Where(s => s.TenSv.Contains(searchString) || s.MaSv.Contains(searchString));
            }

            // Lọc theo lớp học
            if (!string.IsNullOrEmpty(maLop))
            {
                danhSachSV = danhSachSV.Where(s => s.MaLop == maLop);
            }

            return View(await danhSachSV.OrderBy(s => s.MaSv).ToListAsync());
        }

        // GET: /Admin/SinhVien/Create
        public IActionResult Create()
        {
            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa");
            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop");
            ViewBag.MaQue = new SelectList(_context.Ques, "MaQue", "TenQue");
            ViewBag.MaDanToc = new SelectList(_context.DanTocs, "MaDanToc", "TenDanToc");
            ViewBag.MaChuyenNganh = new SelectList(_context.KhoaChuyenNganhs, "MaChuyenNganh", "TenChuyenNganh");
            ViewBag.MaHeDT = new SelectList(_context.HeDaoTaos, "MaHeDT", "TenHeDT");
            ViewBag.MaChucVu = new SelectList(_context.ChucVus, "MaChucVu", "TenChucVu");
            return View();
        }

        // POST: /Admin/SinhVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm mới sinh viên thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop", sinhVien.MaLop);
            ViewBag.MaQue = new SelectList(_context.Ques, "MaQue", "TenQue", sinhVien.MaQue);
            ViewBag.MaDanToc = new SelectList(_context.DanTocs, "MaDanToc", "TenDanToc", sinhVien.MaDanToc);
            ViewBag.MaChuyenNganh = new SelectList(_context.KhoaChuyenNganhs, "MaChuyenNganh", "TenChuyenNganh", sinhVien.MaChuyenNganh);
            ViewBag.MaHeDT = new SelectList(_context.HeDaoTaos, "MaHeDT", "TenHeDT", sinhVien.MaHeDt);
            ViewBag.MaChucVu = new SelectList(_context.ChucVus, "MaChucVu", "TenChucVu", sinhVien.MaChucVu);
            return View(sinhVien);
        }

        // GET: /Admin/SinhVien/Edit/SV001
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null) return NotFound();

            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop", sinhVien.MaLop);
            ViewBag.MaQue = new SelectList(_context.Ques, "MaQue", "TenQue", sinhVien.MaQue);
            ViewBag.MaDanToc = new SelectList(_context.DanTocs, "MaDanToc", "TenDanToc", sinhVien.MaDanToc);
            ViewBag.MaChuyenNganh = new SelectList(_context.KhoaChuyenNganhs, "MaChuyenNganh", "TenChuyenNganh", sinhVien.MaChuyenNganh);
            ViewBag.MaHeDT = new SelectList(_context.HeDaoTaos, "MaHeDT", "TenHeDT", sinhVien.MaHeDt);
            ViewBag.MaChucVu = new SelectList(_context.ChucVus, "MaChucVu", "TenChucVu", sinhVien.MaChucVu);
            return View(sinhVien);
        }

        // POST: /Admin/SinhVien/Edit/SV001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SinhVien sinhVien)
        {
            if (id != sinhVien.MaSv) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin sinh viên thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SinhViens.Any(e => e.MaSv == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop", sinhVien.MaLop);
            ViewBag.MaQue = new SelectList(_context.Ques, "MaQue", "TenQue", sinhVien.MaQue);
            ViewBag.MaDanToc = new SelectList(_context.DanTocs, "MaDanToc", "TenDanToc", sinhVien.MaDanToc);
            ViewBag.MaChuyenNganh = new SelectList(_context.KhoaChuyenNganhs, "MaChuyenNganh", "TenChuyenNganh", sinhVien.MaChuyenNganh);
            ViewBag.MaHeDT = new SelectList(_context.HeDaoTaos, "MaHeDT", "TenHeDT", sinhVien.MaHeDt);
            ViewBag.MaChucVu = new SelectList(_context.ChucVus, "MaChucVu", "TenChucVu", sinhVien.MaChucVu);
            return View(sinhVien);
        }

        // POST: /Admin/SinhVien/Delete/SV001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa sinh viên thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
