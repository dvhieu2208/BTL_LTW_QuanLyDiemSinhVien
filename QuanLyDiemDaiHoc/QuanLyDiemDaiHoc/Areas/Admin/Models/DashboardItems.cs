namespace QuanLyDiemDaiHoc.Areas.Admin.Models
{
    /// <summary>
    /// Một hoạt động gần đây hiển thị trên Dashboard (lấy từ bảng DangKyHocPhan).
    /// Chỉ dùng cho hiển thị, không ánh xạ vào database.
    /// </summary>
    public class HoatDongGanDayItem
    {
        public string MaSv { get; set; } = string.Empty;
        public string TenSv { get; set; } = string.Empty;
        public string MaMon { get; set; } = string.Empty;
        public string TenMon { get; set; } = string.Empty;
        public string MaLop { get; set; } = string.Empty;
        public DateTime ThoiGian { get; set; }
    }

    /// <summary>
    /// Cặp (tên nhóm, số lượng) dùng cho biểu đồ thống kê trên Dashboard.
    /// </summary>
    public class ThongKeNhomItem
    {
        public string Ten { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }
}
