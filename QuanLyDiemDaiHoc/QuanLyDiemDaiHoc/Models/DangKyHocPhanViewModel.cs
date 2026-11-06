using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models
{
    public class DangKyHocPhanViewModel
    {
        public List<ThoiKhoaBieu> DanhSachHocPhan { get; set; }
            = new List<ThoiKhoaBieu>();

        public HashSet<string> MaMonDaDangKy { get; set; }
            = new HashSet<string>();

        // Chỉ những đăng ký được tạo trong đợt hiện tại mới được phép hủy.
        public HashSet<string> MaMonDuocHuy { get; set; }
            = new HashSet<string>();

        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public bool DaCoCauHinh { get; set; }
        public bool DangTrongThoiHan { get; set; }
        public bool DaHetHan { get; set; }
    }
}
