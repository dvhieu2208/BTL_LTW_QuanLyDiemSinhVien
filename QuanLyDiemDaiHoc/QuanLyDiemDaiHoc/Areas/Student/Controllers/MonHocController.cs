using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

namespace QuanLyDiemDaiHoc.Areas.Student.Controllers
{
    [Area("Student")]
    public class MonHocController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public MonHocController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        private async Task<CauHinhDangKyHocPhan?> LayCauHinh()
        {
            return await _context.CauHinhDangKyHocPhans
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == 1);
        }

        private static bool DangTrongThoiHan(CauHinhDangKyHocPhan? cauHinh)
        {
            if (cauHinh == null) return false;

            var hienTai = DateTime.Now;
            return hienTai >= cauHinh.NgayBatDau &&
                   hienTai < cauHinh.NgayKetThuc.AddMinutes(1);
        }

        // HIỂN THỊ DANH SÁCH HỌC PHẦN
        public async Task<IActionResult> Index()
        {
            string? maSv = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var sinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(s => s.MaSv == maSv);

            if (sinhVien == null || string.IsNullOrEmpty(sinhVien.MaLop))
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin lớp của sinh viên.";
                return View(new DangKyHocPhanViewModel());
            }

            var cauHinh = await LayCauHinh();
            var hienTai = DateTime.Now;
            bool daHetHan = cauHinh != null && hienTai >= cauHinh.NgayKetThuc.AddMinutes(1);
            bool dangTrongThoiHan = DangTrongThoiHan(cauHinh);

            // Khi hết hạn: chỉ lấy các học phần sinh viên đã đăng ký.
            // Không xóa dữ liệu các học phần chưa đăng ký khỏi database.
            var danhSach = await _context.ThoiKhoaBieus
                .Include(t => t.MaMonNavigation)
                .Include(t => t.MaPhongNavigation)
                .Where(t => t.MaLop == sinhVien.MaLop)
                .Where(t => !daHetHan ||
                    _context.DangKyHocPhans.Any(d =>
                        d.MaSv == maSv &&
                        d.MaLop == t.MaLop &&
                        d.MaMon == t.MaMon))
                .ToListAsync();

            var cacDangKy = await _context.DangKyHocPhans
                .Where(d => d.MaSv == maSv && d.MaLop == sinhVien.MaLop)
                .Select(d => new { d.MaMon, d.NgayDangKy })
                .ToListAsync();

            var daDangKy = cacDangKy
                .Select(d => d.MaMon)
                .ToHashSet();

            // Đăng ký từ đợt trước vẫn được giữ lại nhưng không được hủy ở đợt sau.
            var duocHuy = cauHinh == null
                ? new HashSet<string>()
                : cacDangKy
                    .Where(d => d.NgayDangKy >= cauHinh.NgayBatDau &&
                                d.NgayDangKy < cauHinh.NgayKetThuc.AddMinutes(1))
                    .Select(d => d.MaMon)
                    .ToHashSet();

            var model = new DangKyHocPhanViewModel
            {
                DanhSachHocPhan = danhSach,
                MaMonDaDangKy = daDangKy,
                MaMonDuocHuy = duocHuy,
                DaCoCauHinh = cauHinh != null,
                NgayBatDau = cauHinh?.NgayBatDau,
                NgayKetThuc = cauHinh?.NgayKetThuc,
                DangTrongThoiHan = dangTrongThoiHan,
                DaHetHan = daHetHan
            };

            return View(model);
        }

        // ĐĂNG KÝ HỌC PHẦN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(string maMon)
        {
            string? maSv = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var cauHinh = await LayCauHinh();
            if (!DangTrongThoiHan(cauHinh))
            {
                TempData["ThongBao"] = cauHinh == null
                    ? "Nhà trường chưa thiết lập thời hạn đăng ký học phần."
                    : DateTime.Now < cauHinh.NgayBatDau
                        ? "Chưa đến thời gian mở đăng ký học phần."
                        : "Đã hết thời hạn đăng ký học phần. Vui lòng chờ đợt tiếp theo.";

                return RedirectToAction(nameof(Index));
            }

            var sinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(s => s.MaSv == maSv);

            if (sinhVien == null || string.IsNullOrEmpty(sinhVien.MaLop))
            {
                TempData["ThongBao"] = "Không tìm thấy thông tin sinh viên.";
                return RedirectToAction(nameof(Index));
            }

            var monHoc = await _context.ThoiKhoaBieus
                .FirstOrDefaultAsync(t =>
                    t.MaLop == sinhVien.MaLop && t.MaMon == maMon);

            if (monHoc == null)
            {
                TempData["ThongBao"] = "Học phần không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var chiTietMon = await _context.MonHocs.FirstOrDefaultAsync(m => m.MaMon == maMon);
            if (chiTietMon != null && chiTietMon.SoLuongToiDa > 0)
            {
                int soLuongHienTai = await _context.DangKyHocPhans
                    .CountAsync(d => d.MaMon == maMon);

                if (soLuongHienTai >= chiTietMon.SoLuongToiDa)
                {
                    TempData["ThongBao"] = $"Học phần này đã đủ số lượng tối đa ({chiTietMon.SoLuongToiDa} sinh viên). Không thể đăng ký thêm.";
                    return RedirectToAction(nameof(Index));
                }
            }
         

            bool daCoDiem = await _context.Diems.AnyAsync(d =>
                d.MaSv == maSv && d.MaLop == sinhVien.MaLop && d.MaMon == maMon);

            if (daCoDiem)
            {
                TempData["ThongBao"] = "Bạn đã có điểm môn học này, không thể đăng ký lại.";
                return RedirectToAction(nameof(Index));
            }

            bool daDangKy = await _context.DangKyHocPhans.AnyAsync(d =>
                d.MaSv == maSv &&
                d.MaLop == sinhVien.MaLop &&
                d.MaMon == maMon);

            if (daDangKy)
            {
                TempData["ThongBao"] = "Bạn đã đăng ký học phần này rồi.";
                return RedirectToAction(nameof(Index));
            }

            _context.DangKyHocPhans.Add(new DangKyHocPhan
            {
                MaSv = maSv,
                MaLop = sinhVien.MaLop,
                MaMon = maMon,
                NgayDangKy = DateTime.Now
            });

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đăng ký học phần thành công!";
            return RedirectToAction(nameof(Index));
        }

        // HỦY ĐĂNG KÝ HỌC PHẦN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyDangKy(string maMon)
        {
            string? maSv = HttpContext.Session.GetString("MaSV");

            if (string.IsNullOrEmpty(maSv))
                return RedirectToAction("Login", "Account", new { area = "" });

            var cauHinh = await LayCauHinh();
            if (!DangTrongThoiHan(cauHinh))
            {
                TempData["ThongBao"] = cauHinh == null
                    ? "Nhà trường chưa thiết lập thời hạn đăng ký học phần."
                    : DateTime.Now < cauHinh.NgayBatDau
                        ? "Chưa đến thời gian mở đăng ký, không thể hủy học phần."
                        : "Đã hết thời hạn đăng ký. Bạn không thể hủy học phần nữa.";

                return RedirectToAction(nameof(Index));
            }

            var sinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(s => s.MaSv == maSv);

            if (sinhVien == null || string.IsNullOrEmpty(sinhVien.MaLop))
                return RedirectToAction(nameof(Index));

            var dangKy = await _context.DangKyHocPhans
                .FirstOrDefaultAsync(d =>
                    d.MaSv == maSv &&
                    d.MaLop == sinhVien.MaLop &&
                    d.MaMon == maMon);

            if (dangKy == null)
            {
                TempData["ThongBao"] = "Không tìm thấy học phần cần hủy.";
                return RedirectToAction(nameof(Index));
            }

            // Chỉ cho hủy đăng ký được tạo trong chính đợt đang mở.
            // Đăng ký từ đợt trước đã khóa vĩnh viễn, kể cả khi mở đợt mới.
            if (dangKy.NgayDangKy < cauHinh!.NgayBatDau ||
                dangKy.NgayDangKy >= cauHinh.NgayKetThuc.AddMinutes(1))
            {
                TempData["ThongBao"] =
                    "Học phần này đã đăng ký ở đợt trước nên không thể hủy.";
                return RedirectToAction(nameof(Index));
            }

            _context.DangKyHocPhans.Remove(dangKy);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Đã hủy đăng ký học phần thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
