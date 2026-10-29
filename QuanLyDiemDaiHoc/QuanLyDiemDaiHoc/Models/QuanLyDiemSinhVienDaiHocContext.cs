using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyDiemDaiHoc.Models;

public partial class QuanLyDiemSinhVienDaiHocContext : DbContext
{
    public QuanLyDiemSinhVienDaiHocContext()
    {
    }

    public QuanLyDiemSinhVienDaiHocContext(DbContextOptions<QuanLyDiemSinhVienDaiHocContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChucVu> ChucVus { get; set; }

    public virtual DbSet<DanToc> DanTocs { get; set; }

    public virtual DbSet<Diem> Diems { get; set; }

    public virtual DbSet<HeDaoTao> HeDaoTaos { get; set; }

    public virtual DbSet<Khoa> Khoas { get; set; }

    public virtual DbSet<KhoaChuyenNganh> KhoaChuyenNganhs { get; set; }

    public virtual DbSet<Lop> Lops { get; set; }

    public virtual DbSet<MonHoc> MonHocs { get; set; }

    public virtual DbSet<PhongHoc> PhongHocs { get; set; }

    public virtual DbSet<Que> Ques { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThoiKhoaBieu> ThoiKhoaBieus { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=QuanLyDiemSinhVienDaiHoc;User ID=sa;Password=123456;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChucVu>(entity =>
        {
            entity.HasKey(e => e.MaChucVu).HasName("PK__ChucVu__D4639533092CF544");

            entity.ToTable("ChucVu");

            entity.Property(e => e.MaChucVu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenChucVu).HasMaxLength(100);
        });

        modelBuilder.Entity<DanToc>(entity =>
        {
            entity.HasKey(e => e.MaDanToc).HasName("PK__DanToc__A5FA0970FA168F88");

            entity.ToTable("DanToc");

            entity.Property(e => e.MaDanToc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenDanToc).HasMaxLength(50);
        });

        modelBuilder.Entity<Diem>(entity =>
        {
            entity.HasKey(e => new { e.MaSv, e.MaLop, e.MaMon });

            entity.ToTable("Diem");

            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaMon)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Diem1)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("Diem");

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.Diems)
                .HasForeignKey(d => d.MaLop)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Diem_Lop");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.Diems)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Diem_Mon");

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.Diems)
                .HasForeignKey(d => d.MaSv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Diem_SV");
        });

        modelBuilder.Entity<HeDaoTao>(entity =>
        {
            entity.HasKey(e => e.MaHeDt).HasName("PK__HeDaoTao__0C0AE7944870AC48");

            entity.ToTable("HeDaoTao");

            entity.Property(e => e.MaHeDt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaHeDT");
            entity.Property(e => e.TenHeDt)
                .HasMaxLength(100)
                .HasColumnName("TenHeDT");
        });

        modelBuilder.Entity<Khoa>(entity =>
        {
            entity.HasKey(e => e.MaKhoa).HasName("PK__Khoa__65390405B9161D16");

            entity.ToTable("Khoa");

            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.TenKhoa).HasMaxLength(100);
            entity.Property(e => e.Website)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<KhoaChuyenNganh>(entity =>
        {
            entity.HasKey(e => e.MaChuyenNganh).HasName("PK__Khoa_Chu__20FEA98DFDE2636F");

            entity.ToTable("Khoa_ChuyenNganh");

            entity.Property(e => e.MaChuyenNganh)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenChuyenNganh).HasMaxLength(100);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.KhoaChuyenNganhs)
                .HasForeignKey(d => d.MaKhoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KCN_Khoa");
        });

        modelBuilder.Entity<Lop>(entity =>
        {
            entity.HasKey(e => e.MaLop).HasName("PK__Lop__3B98D273F1A9177F");

            entity.ToTable("Lop");

            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.KhoaHoc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.SiSo).HasDefaultValue(0);
            entity.Property(e => e.TenLop).HasMaxLength(100);

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.Lops)
                .HasForeignKey(d => d.MaKhoa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lop_Khoa");
        });

        modelBuilder.Entity<MonHoc>(entity =>
        {
            entity.HasKey(e => e.MaMon).HasName("PK__MonHoc__3A5B29A8091F598B");

            entity.ToTable("MonHoc");

            entity.Property(e => e.MaMon)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Dvht).HasColumnName("DVHT");
            entity.Property(e => e.TenMon).HasMaxLength(100);
        });

        modelBuilder.Entity<PhongHoc>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__PhongHoc__20BD5E5BA4DEB9E9");

            entity.ToTable("PhongHoc");

            entity.Property(e => e.MaPhong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenPhong).HasMaxLength(100);
        });

        modelBuilder.Entity<Que>(entity =>
        {
            entity.HasKey(e => e.MaQue).HasName("PK__Que__39A2C8EAF98BC235");

            entity.ToTable("Que");

            entity.Property(e => e.MaQue)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenQue).HasMaxLength(100);
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSv).HasName("PK__SinhVien__2725081A320365A5");

            entity.ToTable("SinhVien");

            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.GioiTinh).HasMaxLength(5);
            entity.Property(e => e.MaChucVu)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaChuyenNganh)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaDanToc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaHeDt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaHeDT");
            entity.Property(e => e.MaKhoa)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaQue)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenSv)
                .HasMaxLength(100)
                .HasColumnName("TenSV");

            entity.HasOne(d => d.MaChucVuNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaChucVu)
                .HasConstraintName("FK_SV_ChucVu");

            entity.HasOne(d => d.MaChuyenNganhNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaChuyenNganh)
                .HasConstraintName("FK_SV_ChuyenNganh");

            entity.HasOne(d => d.MaDanTocNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaDanToc)
                .HasConstraintName("FK_SV_DanToc");

            entity.HasOne(d => d.MaHeDtNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaHeDt)
                .HasConstraintName("FK_SV_HeDT");

            entity.HasOne(d => d.MaKhoaNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaKhoa)
                .HasConstraintName("FK_SV_Khoa");

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaLop)
                .HasConstraintName("FK_SV_Lop");

            entity.HasOne(d => d.MaQueNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaQue)
                .HasConstraintName("FK_SV_Que");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TenDangNhap).HasName("PK__TaiKhoan__55F68FC1EA7D538E");

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaSv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MaSV");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaSvNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaSv)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_DangNhap_SinhVien");
        });

        modelBuilder.Entity<ThoiKhoaBieu>(entity =>
        {
            entity.HasKey(e => new { e.MaLop, e.MaMon }).HasName("PK_TKB");

            entity.ToTable("ThoiKhoaBieu");

            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaMon)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaPhong)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.ThoiKhoaBieus)
                .HasForeignKey(d => d.MaLop)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TKB_Lop");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.ThoiKhoaBieus)
                .HasForeignKey(d => d.MaMon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TKB_Mon");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.ThoiKhoaBieus)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK_TKB_Phong");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
