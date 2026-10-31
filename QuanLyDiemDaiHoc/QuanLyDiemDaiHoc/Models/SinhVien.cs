using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace QuanLyDiemDaiHoc.Models;

public partial class SinhVien
{
    public string MaSv { get; set; } = null!;

    public string TenSv { get; set; } = null!;

    public string? MaKhoa { get; set; }

    public string? MaLop { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? MaQue { get; set; }

    public string? MaDanToc { get; set; }

    public string? MaChuyenNganh { get; set; }

    public string? MaHeDt { get; set; }

    public string? MaChucVu { get; set; }
    [ValidateNever]
    public virtual ICollection<Diem> Diems { get; set; } = new List<Diem>();
    [ValidateNever]

    public virtual ChucVu? MaChucVuNavigation { get; set; }
    [ValidateNever]
    public virtual KhoaChuyenNganh? MaChuyenNganhNavigation { get; set; }
    [ValidateNever]
    public virtual DanToc? MaDanTocNavigation { get; set; }
    [ValidateNever]

    public virtual HeDaoTao? MaHeDtNavigation { get; set; }
    [ValidateNever]
    public virtual Khoa? MaKhoaNavigation { get; set; }
    [ValidateNever]

    public virtual Lop? MaLopNavigation { get; set; }
    [ValidateNever]

    public virtual Que? MaQueNavigation { get; set; }
    [ValidateNever]
    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
