using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyDiemDaiHoc.Models;

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
            public async Task<IActionResult> Index(string maLop, string maMon, int? hocKy)
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

                // Lấy danh sách sinh viên thuộc lớp được chọn
                var danhSachSV = await _context.SinhViens
                    .Where(s => s.MaLop == maLop)
                    .OrderBy(s => s.MaSv)
                    .ToListAsync();

                // Lấy danh sách điểm hiện có của lớp + môn này
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
                        LanThi = diemSV?.LanThi ?? 1
                    };
                }).ToList();

                return View(model);
            }

            // POST: /Admin/Diem/SaveGrades
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> SaveGrades(string maLop, string maMon, int hocKy, List<DiemModelItem> items)
            {
                if (string.IsNullOrEmpty(maLop) || string.IsNullOrEmpty(maMon))
                {
                    TempData["Error"] = "Vui lòng chọn lớp và môn học!";
                    return RedirectToAction("Index");
                }

                foreach (var item in items)
                {
                    if (item.Diem.HasValue)
                    {
                        // Kiểm tra ràng buộc điểm từ 0 đến 10
                        if (item.Diem < 0 || item.Diem > 10) continue;

                        var diemExisting = await _context.Diems.FirstOrDefaultAsync(d => d.MaSv == item.MaSv && d.MaLop == maLop && d.MaMon == maMon);

                        if (diemExisting != null)
                        {
                            // Cập nhật điểm đã tồn tại
                            diemExisting.Diem1 = item.Diem;
                            diemExisting.HocKy = hocKy;
                            diemExisting.LanThi = item.LanThi;
                            _context.Update(diemExisting);
                        }
                        else
                        {
                            // Thêm điểm mới
                            var diemMoi = new Diem
                            {
                                MaSv = item.MaSv,
                                MaLop = maLop,
                                MaMon = maMon,
                                HocKy = hocKy,
                                LanThi = item.LanThi,
                                Diem1 = item.Diem
                            };
                            _context.Add(diemMoi);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Lưu điểm thành công!";

                return RedirectToAction("Index", new { maLop = maLop, maMon = maMon, hocKy = hocKy });
            }
        }

        // ViewModel hỗ trợ nhập điểm theo từng sinh viên
        public class DiemModelItem
        {
            public string MaSv { get; set; } = string.Empty;
            public string TenSv { get; set; } = string.Empty;
            public decimal? Diem { get; set; }
            public int LanThi { get; set; } = 1;
        }
    }
