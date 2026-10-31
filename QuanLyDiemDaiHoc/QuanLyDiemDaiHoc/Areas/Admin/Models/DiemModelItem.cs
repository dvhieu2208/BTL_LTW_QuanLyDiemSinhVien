namespace QuanLyDiemDaiHoc.Areas.Admin.Models
{
    public class DiemModelItem
    {
            public string MaSv { get; set; } = string.Empty;
            public string TenSv { get; set; } = string.Empty;
            public decimal? Diem { get; set; }
            public int LanThi { get; set; } = 1;
    }
}
