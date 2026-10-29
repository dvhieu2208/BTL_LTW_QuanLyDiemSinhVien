using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class ThoiKhoaBieu
{
    public string MaLop { get; set; } = null!;

    public string MaMon { get; set; } = null!;

    public int? HocKy { get; set; }

    public int? ThuHoc { get; set; }

    public int? CaHoc { get; set; }

    public string? MaPhong { get; set; }

    public virtual Lop MaLopNavigation { get; set; } = null!;

    public virtual MonHoc MaMonNavigation { get; set; } = null!;

    public virtual PhongHoc? MaPhongNavigation { get; set; }
}
