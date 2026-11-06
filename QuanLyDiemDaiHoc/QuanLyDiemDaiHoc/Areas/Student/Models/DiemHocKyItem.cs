namespace QuanLyDiemDaiHoc.Areas.Student.Models
{
    /// <summary>
    /// Điểm trung bình theo học kỳ, dùng cho biểu đồ trên trang chủ sinh viên.
    /// Chỉ phục vụ hiển thị, không ánh xạ vào database.
    /// </summary>
    public class DiemHocKyItem
    {
        public int HocKy { get; set; }
        public double DiemTrungBinh { get; set; }
        public int SoMon { get; set; }
    }
}
