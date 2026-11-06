using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

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
        public async Task<IActionResult> Index(string searchString, int? thu, int? ca)
        {
            // Nạp dữ liệu cho Dropdown Thứ và Ca
            ViewBag.DanhSachThu = new SelectList(new List<object> {
                new { Id = 2, Name = "Thứ Hai" },
                new { Id = 3, Name = "Thứ Ba" },
                new { Id = 4, Name = "Thứ Tư" },
                new { Id = 5, Name = "Thứ Năm" },
                new { Id = 6, Name = "Thứ Sáu" },
                new { Id = 7, Name = "Thứ Bảy" },
                new { Id = 8, Name = "Chủ Nhật" }
            }, "Id", "Name", thu);

            ViewBag.DanhSachCa = new SelectList(new List<object> {
                new { Id = 1, Name = "Ca 1 (7:30 - 9:30)" },
                new { Id = 2, Name = "Ca 2 (9:45 - 11:45)" },
                new { Id = 3, Name = "Ca 3 (13:30 - 15:30)" },
                new { Id = 4, Name = "Ca 4 (15:45 - 17:45)" }
            }, "Id", "Name", ca);

            ViewBag.SearchString = searchString;
            ViewBag.SelectedThu = thu;
            ViewBag.SelectedCa = ca;

            // Truy vấn danh sách phòng học kết hợp tìm kiếm từ khóa
            var query = _context.PhongHocs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p => p.MaPhong.Contains(searchString) || p.TenPhong.Contains(searchString));
            }

            var danhSachPhong = await query.ToListAsync();

            // Kiểm tra trạng thái Trống / Bận dựa theo Thứ và Ca (nếu người dùng có chọn)
            var model = new List<PhongHocTrangThaiViewModel>();

            foreach (var phong in danhSachPhong)
            {
                string trangThai = "Sẵn sàng";
                string chiTietBan = "";

                if (thu.HasValue && ca.HasValue)
                {
                    // Kiểm tra trong bảng ThoiKhoaBieu xem phòng đã có lịch vào Thứ và Ca này chưa
                    // (Lưu ý: Tên thuộc tính MaPhong, ThuHoc, CaHoc trong ThoiKhoaBieu tùy thuộc vào DbContext của bạn)
                    var lichHoc = await _context.ThoiKhoaBieus
                        .FirstOrDefaultAsync(tkb => tkb.MaPhong == phong.MaPhong && tkb.ThuHoc == thu && tkb.CaHoc == ca);

                    if (lichHoc != null)
                    {
                        trangThai = "Đang bận";
                        chiTietBan = $"Môn: {lichHoc.MaMon}";
                    }
                }

                model.Add(new PhongHocTrangThaiViewModel
                {
                    MaPhong = phong.MaPhong,
                    TenPhong = phong.TenPhong,
                    TrangThai = trangThai,
                    ChiTietBan = chiTietBan
                });
            }

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
                _context.PhongHocs.Remove(phongHoc);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa phòng học thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }

    // ViewModel hỗ trợ hiển thị trạng thái phòng học
    public class PhongHocTrangThaiViewModel
    {
        public string MaPhong { get; set; } = null!;
        public string TenPhong { get; set; } = null!;
        public string TrangThai { get; set; } = "Sẵn sàng";
        public string ChiTietBan { get; set; } = string.Empty;
    }
}