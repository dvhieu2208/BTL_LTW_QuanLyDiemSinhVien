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

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.MaKhoa = new SelectList(_context.Khoas ?? Enumerable.Empty<Khoa>(), "MaKhoa", "TenKhoa");
            ViewBag.MaLop = new SelectList(_context.Lops ?? Enumerable.Empty<Lop>(), "MaLop", "TenLop");
            ViewBag.MaQue = new SelectList(_context.Ques ?? Enumerable.Empty<Que>(), "MaQue", "TenQue");
            ViewBag.MaDanToc = new SelectList(_context.DanTocs ?? Enumerable.Empty<DanToc>(), "MaDanToc", "TenDanToc");
            ViewBag.MaHeDt = new SelectList(_context.HeDaoTaos ?? Enumerable.Empty<HeDaoTao>(), "MaHeDt", "TenHeDt");
            ViewBag.MaChuyenNganh = new SelectList(_context.KhoaChuyenNganhs ?? Enumerable.Empty<KhoaChuyenNganh>(), "MaChuyenNganh", "TenChuyenNganh");
            ViewBag.MaChucVu = new SelectList(_context.ChucVus ?? Enumerable.Empty<ChucVu>(), "MaChucVu", "TenChucVu");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SinhVien sv)
        {
            if (ModelState.IsValid)
            {
                // 1. Thêm mới sinh viên vào database
                _context.SinhViens.Add(sv);

                // 2. Tự động khởi tạo tài khoản đăng nhập tương ứng cho sinh viên vừa thêm
                // (Mã sinh viên làm tên đăng nhập, mật khẩu mặc định là "123456")
                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = sv.MaSv,
                    MatKhau = "123456", // Hoặc mã hóa mật khẩu nếu hệ thống có hàm hash
                    PhanQuyen = 1,
                    MaSv = sv.MaSv
                };
                _context.TaiKhoans.Add(taiKhoan);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm sinh viên và cấp tài khoản thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaKhoa = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sv.MaKhoa);
            ViewBag.MaLop = new SelectList(_context.Lops, "MaLop", "TenLop", sv.MaLop);
            ViewBag.MaQue = new SelectList(_context.Ques, "MaQue", "TenQue", sv.MaQue);
            ViewBag.MaDanToc = new SelectList(_context.DanTocs, "MaDanToc", "TenDanToc", sv.MaDanToc);
            ViewBag.MaHeDt = new SelectList(_context.HeDaoTaos, "MaHeDt", "TenHeDt", sv.MaHeDt);
            ViewBag.MaChuyenNganh = new SelectList(_context.KhoaChuyenNganhs, "MaChuyenNganh", "TenChuyenNganh", sv.MaChuyenNganh);
            ViewBag.MaChucVu = new SelectList(_context.ChucVus, "MaChucVu", "TenChucVu", sv.MaChucVu);

            return View(sv);
        }

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sinhVien = await _context.SinhViens
                .Include(s => s.TaiKhoans) // Include bảng tài khoản liên quan
                .Include(s => s.Diems)     // Include cả bảng Điểm liên quan (tên navigation property của bạn có thể là Diems hoặc Diem)
                .FirstOrDefaultAsync(s => s.MaSv == id);

            if (sinhVien != null)
            {
                // 1. Xóa tất cả các bản ghi điểm của sinh viên này trước (nếu có)
                if (sinhVien.Diems != null && sinhVien.Diems.Any())
                {
                    _context.Diems.RemoveRange(sinhVien.Diems);
                }

                // 2. Xóa tài khoản đăng nhập liên quan (nếu có)
                var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == sinhVien.MaSv);
                if (taiKhoan != null)
                {
                    // Xóa tài khoản nếu tồn tại
                    _context.TaiKhoans.Remove(taiKhoan);
                }
                // 3. Sau đó mới tiến hành xóa sinh viên
                _context.SinhViens.Remove(sinhVien);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa sinh viên và các dữ liệu liên quan thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> GetLopByKhoa(string maKhoa)
        {
            var lops = await _context.Lops
                .Where(l => l.MaKhoa == maKhoa)
                .Select(l => new { maLop = l.MaLop, tenLop = l.TenLop })
                .ToListAsync();
            return Json(lops);
        }

        [HttpGet]
        public async Task<IActionResult> GetChuyenNganhByKhoa(string maKhoa)
        {
            var chuyenNganhs = await _context.KhoaChuyenNganhs
                .Where(c => c.MaKhoa == maKhoa)
                .Select(c => new { maChuyenNganh = c.MaChuyenNganh, tenChuyenNganh = c.TenChuyenNganh })
                .ToListAsync();
            return Json(chuyenNganhs);
        }
    }
}
