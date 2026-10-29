using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class Khoa
{
    public string MaKhoa { get; set; } = null!;

    public string TenKhoa { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? Website { get; set; }

    public virtual ICollection<KhoaChuyenNganh> KhoaChuyenNganhs { get; set; } = new List<KhoaChuyenNganh>();

    public virtual ICollection<Lop> Lops { get; set; } = new List<Lop>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
