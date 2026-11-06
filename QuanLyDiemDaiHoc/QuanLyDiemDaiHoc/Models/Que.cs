using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class Que
{
    public string MaQue { get; set; } = null!;

    public string TenQue { get; set; } = null!;

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
