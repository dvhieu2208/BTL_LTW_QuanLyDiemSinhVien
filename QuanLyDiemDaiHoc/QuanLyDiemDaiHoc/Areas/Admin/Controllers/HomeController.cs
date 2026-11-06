using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;
using QuanLyDiemDaiHoc.Areas.Admin.Models;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public HomeController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            int tongSinhVien = await _context.SinhViens.CountAsync();
            ViewBag.TongSinhVien = tongSinhVien;
            ViewBag.TongLop = await _context.Lops.CountAsync();
            ViewBag.TongMonHoc = await _context.MonHocs.CountAsync();
            ViewBag.TongKhoa = await _context.Khoas.CountAsync();

            // ===== Bổ sung cho Dashboard (chỉ đọc, lấy trực tiếp từ database) =====
            ViewBag.TongPhongHoc = await _context.PhongHocs.CountAsync();
            ViewBag.TongChuyenNganh = await _context.KhoaChuyenNganhs.CountAsync();
            ViewBag.TongLichHoc = await _context.ThoiKhoaBieus.CountAsync();
            ViewBag.TongDangKy = await _context.DangKyHocPhans.CountAsync();
            ViewBag.TongBanGhiDiem = await _context.Diems.CountAsync();
            ViewBag.TaiKhoanHoatDong = await _context.TaiKhoans.CountAsync(t => t.TrangThai);
            ViewBag.TaiKhoanBiKhoa = await _context.TaiKhoans.CountAsync(t => !t.TrangThai);

            ViewBag.SinhVienTheoKhoa = await _context.Khoas
                .AsNoTracking()
                .Select(k => new ThongKeNhomItem
                {
                    Ten = k.TenKhoa,
                    SoLuong = _context.SinhViens.Count(sv => sv.MaKhoa == k.MaKhoa)
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            ViewBag.HoatDongGanDay = await _context.DangKyHocPhans
                .AsNoTracking()
                .OrderByDescending(d => d.NgayDangKy)
                .Take(6)
                .Select(d => new HoatDongGanDayItem
                {
                    MaSv = d.MaSv,
                    TenSv = _context.SinhViens.Where(sv => sv.MaSv == d.MaSv).Select(sv => sv.TenSv).FirstOrDefault() ?? d.MaSv,
                    MaMon = d.MaMon,
                    TenMon = _context.MonHocs.Where(m => m.MaMon == d.MaMon).Select(m => m.TenMon).FirstOrDefault() ?? d.MaMon,
                    MaLop = d.MaLop,
                    ThoiGian = d.NgayDangKy
                })
                .ToListAsync();

            var cauHinhDangKy = await _context.CauHinhDangKyHocPhans
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == 1);
            ViewBag.DotDangKyBatDau = cauHinhDangKy?.NgayBatDau;
            ViewBag.DotDangKyKetThuc = cauHinhDangKy?.NgayKetThuc;

            // Lấy danh sách sinh viên kèm điểm trung bình các môn từ bảng Diem
            var danhSachThongKe = await _context.SinhViens
                .Select(sv => new
                {
                    sv.MaSv,
                    sv.TenSv,
                    sv.MaLop,
                    DiemTrungBinh = _context.Diems
                        .Where(d => d.MaSv == sv.MaSv && d.Diem1.HasValue)
                        .Average(d => (double?)d.Diem1)
                })
                .ToListAsync();
            ViewBag.CountXuatSac = danhSachThongKe.Count(x => x.DiemTrungBinh.HasValue && x.DiemTrungBinh.Value >= 8.5);
            ViewBag.CountGioi = danhSachThongKe.Count(x => x.DiemTrungBinh.HasValue && x.DiemTrungBinh.Value >= 7.0 && x.DiemTrungBinh.Value < 8.5);
            ViewBag.CountKha = danhSachThongKe.Count(x => x.DiemTrungBinh.HasValue && x.DiemTrungBinh.Value >= 5.5 && x.DiemTrungBinh.Value < 7.0);
            ViewBag.CountTrungBinh = danhSachThongKe.Count(x => x.DiemTrungBinh.HasValue && x.DiemTrungBinh.Value >= 4.0 && x.DiemTrungBinh.Value < 5.5);
            ViewBag.CountYeu = danhSachThongKe.Count(x => x.DiemTrungBinh.HasValue && x.DiemTrungBinh.Value < 4.0);
            ViewBag.CountChuaCoDiem = danhSachThongKe.Count(x => !x.DiemTrungBinh.HasValue);

            ViewBag.SinhVienNoiBat = danhSachThongKe
                .Where(x => x.DiemTrungBinh.HasValue)
                .OrderByDescending(x => x.DiemTrungBinh)
                .Take(5)
                .Select(x => new
                {
                    x.MaSv,
                    x.TenSv,
                    x.MaLop,
                    Gpa = x.DiemTrungBinh!.Value
                })
                .ToList();

            return View();
        }
    }
}
