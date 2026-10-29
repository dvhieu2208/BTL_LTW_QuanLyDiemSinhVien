using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class DanToc
{
    public string MaDanToc { get; set; } = null!;

    public string TenDanToc { get; set; } = null!;

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
