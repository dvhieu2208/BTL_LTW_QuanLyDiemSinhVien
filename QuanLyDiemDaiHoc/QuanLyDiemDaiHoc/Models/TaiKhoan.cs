using System;
using System.Collections.Generic;

namespace QuanLyDiemDaiHoc.Models;

public partial class TaiKhoan
{
    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? MaSv { get; set; }

    public int PhanQuyen { get; set; }

    public bool TrangThai { get; set; }

    public virtual SinhVien? MaSvNavigation { get; set; }
}
