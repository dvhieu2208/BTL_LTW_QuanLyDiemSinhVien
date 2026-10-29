using System;
using System.Collections.Generic;

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

    public virtual ICollection<Diem> Diems { get; set; } = new List<Diem>();

    public virtual ChucVu? MaChucVuNavigation { get; set; }

    public virtual KhoaChuyenNganh? MaChuyenNganhNavigation { get; set; }

    public virtual DanToc? MaDanTocNavigation { get; set; }

    public virtual HeDaoTao? MaHeDtNavigation { get; set; }

    public virtual Khoa? MaKhoaNavigation { get; set; }

    public virtual Lop? MaLopNavigation { get; set; }

    public virtual Que? MaQueNavigation { get; set; }

    public virtual ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
