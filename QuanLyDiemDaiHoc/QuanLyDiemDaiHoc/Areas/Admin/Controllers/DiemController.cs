using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Areas.Admin.Models;
using QuanLyDiemDaiHoc.Models;
using System.Globalization;

namespace QuanLyDiemDaiHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiemController : Controller
    {
        private readonly QuanLyDiemSinhVienDaiHocContext _context;

        public DiemController(QuanLyDiemSinhVienDaiHocContext context)
        {
            _context = context;
        }

        // GET: /Admin/Diem/Index
        public async Task<IActionResult> Index(string maLop, string maMon, int? hocKy, string highlightSv)
        {
            // Kiểm tra session đăng nhập
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Nạp dữ liệu danh sách cho các Dropdown
            ViewBag.MaLop = new SelectList(await _context.Lops.ToListAsync(), "MaLop", "TenLop", maLop);
            ViewBag.MaMon = new SelectList(await _context.MonHocs.ToListAsync(), "MaMon", "TenMon", maMon);

            // Lưu các tham số lọc lại ViewBag
            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedMaMon = maMon;
            ViewBag.SelectedHocKy = hocKy ?? 1;

            if (string.IsNullOrEmpty(maLop) || string.IsNullOrEmpty(maMon))
            {
                return View(new List<DiemModelItem>());
            }

            // Lấy danh sách sinh viên thuộc lớp và ĐÃ ĐĂNG KÝ môn học (Bỏ check HocKy ở bảng đăng ký)
            var danhSachSV = await _context.SinhViens
                .Where(s => s.MaLop == maLop && _context.DangKyHocPhans
                    .Any(dk => dk.MaSv == s.MaSv && dk.MaMon == maMon))
                .OrderBy(s => s.MaSv)
                .ToListAsync();

            // Lấy danh sách điểm hiện có của lớp + môn này theo học kỳ
            var danhSachDiem = await _context.Diems
                .Where(d => d.MaLop == maLop && d.MaMon == maMon && d.HocKy == (hocKy ?? 1)).ToListAsync();

            // Kết hợp thông tin thành Model hiển thị lên bảng nhập điểm
            var model = danhSachSV.Select(sv =>
            {
                var diemSV = danhSachDiem.FirstOrDefault(d => d.MaSv == sv.MaSv);
                return new DiemModelItem
                {
                    MaSv = sv.MaSv,
                    TenSv = sv.TenSv,
                    Diem = diemSV?.Diem1,

                    DiemNhap = diemSV?.Diem1?.ToString("0.##",CultureInfo.InvariantCulture),

                    LanThi = diemSV?.LanThi ?? 1
                };
            }).ToList();

            ViewBag.HighlightSv = highlightSv;

            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveGrades(
            string maLop,
            string maMon,
            int hocKy,
            List<DiemModelItem> items)
        {
            if (string.IsNullOrEmpty(maLop) ||
                string.IsNullOrEmpty(maMon))
            {
                TempData["Error"] = "Vui lòng chọn lớp và môn học!";
                return RedirectToAction("Index");
            }

            if (items == null || !items.Any())
            {
                TempData["Error"] = "Không có dữ liệu sinh viên!";
                return RedirectToAction("Index",
                    new { maLop, maMon, hocKy });
            }

            // Chuyển điểm từ chuỗi sang decimal
            var danhSachDiem = new List<(DiemModelItem Item, decimal Diem)>();

            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item.MaSv))
                {
                    TempData["Error"] = "Mã sinh viên không hợp lệ!";
                    return RedirectToAction("Index",
                        new { maLop, maMon, hocKy });
                }

                // Bỏ qua nếu chưa nhập điểm
                if (string.IsNullOrWhiteSpace(item.DiemNhap))
                    continue;

                // Chấp nhận cả 9.5 và 9,5
                string diemText = item.DiemNhap.Trim().Replace(',', '.');

                if (!decimal.TryParse(
                    diemText,
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture,
                    out decimal diemSo))
                {
                    TempData["Error"] =
                        $"Điểm của sinh viên {item.MaSv} không hợp lệ!";

                    return RedirectToAction("Index",
                        new { maLop, maMon, hocKy });
                }

                if (diemSo < 0 || diemSo > 10)
                {
                    TempData["Error"] =
                        $"Điểm của sinh viên {item.MaSv} phải từ 0 đến 10!";

                    return RedirectToAction("Index",
                        new { maLop, maMon, hocKy });
                }

                if (item.LanThi < 1 || item.LanThi > 5)
                {
                    TempData["Error"] =
                        $"Lần thi của sinh viên {item.MaSv} không hợp lệ!";

                    return RedirectToAction("Index",
                        new { maLop, maMon, hocKy });
                }

                danhSachDiem.Add((item, diemSo));
            }

            if (items.GroupBy(x => x.MaSv).Any(g => g.Count() > 1))
            {
                TempData["Error"] = "Danh sách sinh viên bị trùng!";
                return RedirectToAction("Index",
                    new { maLop, maMon, hocKy });
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var danhSachDiemCu = await _context.Diems
                    .Where(d =>
                        d.MaLop == maLop &&
                        d.MaMon == maMon &&
                        d.HocKy == hocKy)
                    .ToListAsync();

                foreach (var data in danhSachDiem)
                {
                    var item = data.Item;
                    decimal diemSo = data.Diem;

                    var diemExisting = danhSachDiemCu.FirstOrDefault(
                        d => d.MaSv == item.MaSv);

                    if (diemExisting != null)
                    {
                        diemExisting.Diem1 = diemSo;
                        diemExisting.LanThi = item.LanThi;
                    }
                    else
                    {
                        var diemMoi = new Diem
                        {
                            MaSv = item.MaSv,
                            MaLop = maLop,
                            MaMon = maMon,
                            HocKy = hocKy,
                            LanThi = item.LanThi,
                            Diem1 = diemSo
                        };

                        _context.Diems.Add(diemMoi);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Lưu bảng điểm thành công!";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                System.Diagnostics.Debug.WriteLine(ex.ToString());

                TempData["Error"] =
                    "Lưu bảng điểm thất bại! " + ex.Message;
            }

            return RedirectToAction("Index",
                new { maLop, maMon, hocKy });
        }



        // GET: /Admin/Diem/XemBangDiem
        public async Task<IActionResult> XemBangDiem(string maLop, string maMon, int? hocKy)
        {
            if (HttpContext.Session.GetString("TenDangNhap") == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            ViewBag.MaLop = new SelectList(await _context.Lops.ToListAsync(), "MaLop", "TenLop", maLop);
            ViewBag.MaMon = new SelectList(await _context.MonHocs.ToListAsync(), "MaMon", "TenMon", maMon);

            ViewBag.SelectedMaLop = maLop;
            ViewBag.SelectedMaMon = maMon;
            ViewBag.SelectedHocKy = hocKy ?? 1;

            if (string.IsNullOrEmpty(maLop) || string.IsNullOrEmpty(maMon))
            {
                return View(new List<DiemModelItem>());
            }

            var danhSachSV = await _context.SinhViens
                .Where(s => s.MaLop == maLop && _context.DangKyHocPhans
                    .Any(dk => dk.MaSv == s.MaSv && dk.MaMon == maMon))
                .OrderBy(s => s.MaSv)
                .ToListAsync();

            var danhSachDiem = await _context.Diems
                .Where(d => d.MaLop == maLop && d.MaMon == maMon && d.HocKy == (hocKy ?? 1))
                .ToListAsync();

            var model = danhSachSV.Select(sv =>
            {
                var diemSV = danhSachDiem.FirstOrDefault(d => d.MaSv == sv.MaSv);
                return new DiemModelItem
                {
                    MaSv = sv.MaSv,
                    TenSv = sv.TenSv,
                    Diem = diemSV?.Diem1,
                    LanThi = diemSV?.LanThi ?? 1
                };
            }).ToList();

            return View(model);
        }
    }
}