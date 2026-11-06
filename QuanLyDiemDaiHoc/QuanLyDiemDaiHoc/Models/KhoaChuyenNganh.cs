using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class KhoaChuyenNganh
{
    public string MaChuyenNganh { get; set; } = null!;

    public string MaKhoa { get; set; } = null!;

    public string TenChuyenNganh { get; set; } = null!;

    public virtual Khoa MaKhoaNavigation { get; set; } = null!;

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
