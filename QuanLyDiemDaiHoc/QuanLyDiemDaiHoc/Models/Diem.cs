using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class Diem
{
    public string MaSv { get; set; } = null!;

    public string MaLop { get; set; } = null!;

    public string MaMon { get; set; } = null!;

    public int? HocKy { get; set; }

    public int? LanThi { get; set; }

    public decimal? Diem1 { get; set; }

    public virtual Lop MaLopNavigation { get; set; } = null!;

    public virtual MonHoc MaMonNavigation { get; set; } = null!;

    public virtual SinhVien MaSvNavigation { get; set; } = null!;
}
