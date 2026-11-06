
using System;

namespace QuanLyDiemDaiHoc.Models
{
    public class DangKyHocPhan
    {
        public string MaSv { get; set; } = null!;
        public string MaLop { get; set; } = null!;

        public string MaMon { get; set; } = null!;

        public DateTime NgayDangKy { get; set; }

        public int HocKy { get; set; } = 1;

        // Liên kết với sinh viên
        public virtual SinhVien MaSvNavigation { get; set; } = null!;

        // Liên kết với thời khóa biểu
        public virtual ThoiKhoaBieu ThoiKhoaBieuNavigation { get; set; } = null!;
    }
}
