using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class MonHoc
{
    public string MaMon { get; set; } = null!;

    public string TenMon { get; set; } = null!;

    public int? Dvht { get; set; }

    public virtual ICollection<Diem> Diems { get; set; } = new List<Diem>();

    public virtual ICollection<ThoiKhoaBieu> ThoiKhoaBieus { get; set; } = new List<ThoiKhoaBieu>();
}
