using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyDiemDaiHoc.Models;

public partial class Lop
{
    public string MaLop { get; set; } = null!;

    public string TenLop { get; set; } = null!;

    public string MaKhoa { get; set; } = null!;

    public string? KhoaHoc { get; set; }

    public int? SiSo { get; set; }

    public virtual ICollection<Diem> Diems { get; set; } = new List<Diem>();
    [ValidateNever]

    public virtual Khoa MaKhoaNavigation { get; set; } = null!;

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();

    public virtual ICollection<ThoiKhoaBieu> ThoiKhoaBieus { get; set; } = new List<ThoiKhoaBieu>();
}
