using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class PhongHoc
{
    public string MaPhong { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public virtual ICollection<ThoiKhoaBieu> ThoiKhoaBieus { get; set; } = new List<ThoiKhoaBieu>();
}
