using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class HeDaoTao
{
    public string MaHeDt { get; set; } = null!;

    public string TenHeDt { get; set; } = null!;

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
