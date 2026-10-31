
USE QuanLyDiemSinhVienDaiHoc;
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE TABLE Khoa (
    MaKhoa    VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenKhoa   NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15),
    DiaChi    NVARCHAR(200),
    Website   VARCHAR(100)
);
GO

CREATE TABLE MonHoc (
    MaMon  VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL,
    DVHT   INT           
);
GO

CREATE TABLE ChucVu (
    MaChucVu  VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenChucVu NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE DanToc (
    MaDanToc  VARCHAR(10)  NOT NULL PRIMARY KEY,
    TenDanToc NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE Que (
    MaQue  VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenQue NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE HeDaoTao (
    MaHeDT  VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenHeDT NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE PhongHoc (
    MaPhong  VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenPhong NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Khoa_ChuyenNganh (
    MaChuyenNganh  VARCHAR(10)   NOT NULL PRIMARY KEY,
    MaKhoa          VARCHAR(10)   NOT NULL,
    TenChuyenNganh NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_KCN_Khoa FOREIGN KEY (MaKhoa) REFERENCES Khoa(MaKhoa)
);
GO

CREATE TABLE Lop (
    MaLop   VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenLop  NVARCHAR(100) NOT NULL,
    MaKhoa  VARCHAR(10)   NOT NULL,
    KhoaHoc VARCHAR(20),
    SiSo    INT DEFAULT 0,
    CONSTRAINT FK_Lop_Khoa FOREIGN KEY (MaKhoa) REFERENCES Khoa(MaKhoa)
);
GO

CREATE TABLE SinhVien (
    MaSV          VARCHAR(10)   NOT NULL PRIMARY KEY,
    TenSV         NVARCHAR(100) NOT NULL,
    MaKhoa        VARCHAR(10),
    MaLop         VARCHAR(10),
    NgaySinh      DATE,
    GioiTinh      NVARCHAR(5),
    MaQue         VARCHAR(10),
    MaDanToc      VARCHAR(10),
    MaChuyenNganh VARCHAR(10),
    MaHeDT        VARCHAR(10),
    MaChucVu      VARCHAR(10),
    CONSTRAINT FK_SV_Khoa FOREIGN KEY (MaKhoa) REFERENCES Khoa(MaKhoa),
    CONSTRAINT FK_SV_Lop  FOREIGN KEY (MaLop) REFERENCES Lop(MaLop),
    CONSTRAINT FK_SV_Que FOREIGN KEY (MaQue) REFERENCES Que(MaQue),
    CONSTRAINT FK_SV_DanToc FOREIGN KEY (MaDanToc) REFERENCES DanToc(MaDanToc),
    CONSTRAINT FK_SV_ChuyenNganh FOREIGN KEY (MaChuyenNganh) REFERENCES Khoa_ChuyenNganh(MaChuyenNganh),
    CONSTRAINT FK_SV_HeDT FOREIGN KEY (MaHeDT) REFERENCES HeDaoTao(MaHeDT),
    CONSTRAINT FK_SV_ChucVu FOREIGN KEY (MaChucVu) REFERENCES ChucVu(MaChucVu)
);
GO

CREATE TABLE TaiKhoan (
    TenDangNhap VARCHAR(50)  NOT NULL PRIMARY KEY,
    MatKhau     VARCHAR(255) NOT NULL,
    MaSV        VARCHAR(10)  NULL, 
    PhanQuyen   INT NOT NULL,
    TrangThai   BIT          NOT NULL DEFAULT 1,
    CONSTRAINT FK_DangNhap_SinhVien FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV) ON DELETE CASCADE
);
GO

CREATE TABLE Diem (
    MaSV   VARCHAR(10) NOT NULL,
    MaLop  VARCHAR(10) NOT NULL,
    MaMon  VARCHAR(10) NOT NULL,
    HocKy  INT,
    LanThi INT,
    Diem   DECIMAL(4,2) CHECK (Diem BETWEEN 0 AND 10),
    CONSTRAINT PK_Diem PRIMARY KEY (MaSV, MaLop, MaMon),
    CONSTRAINT FK_Diem_SV FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    CONSTRAINT FK_Diem_Lop FOREIGN KEY (MaLop) REFERENCES Lop(MaLop),
    CONSTRAINT FK_Diem_Mon FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon)
);
GO

CREATE TABLE ThoiKhoaBieu (
    MaLop   VARCHAR(10) NOT NULL,
    MaMon   VARCHAR(10) NOT NULL,
    HocKy   INT,
    ThuHoc  INT,          
    CaHoc   INT,
    MaPhong VARCHAR(10),
    CONSTRAINT PK_TKB PRIMARY KEY (MaLop, MaMon),
    CONSTRAINT FK_TKB_Lop FOREIGN KEY (MaLop) REFERENCES Lop(MaLop),
    CONSTRAINT FK_TKB_Mon FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    CONSTRAINT FK_TKB_Phong FOREIGN KEY (MaPhong) REFERENCES PhongHoc(MaPhong)
);
GO

-- 3. Insert du lieu mau

INSERT INTO Khoa (MaKhoa, TenKhoa, DienThoai, DiaChi, Website) VALUES
('CNTT', N'Công nghệ thông tin', '0241234567', N'Nhà A1', 'https://cntt.example.edu.vn'),
('KT', N'Kinh tế', '0241234568', N'Nhà B2', 'https://kinhte.example.edu.vn');
GO

INSERT INTO MonHoc (MaMon, TenMon, DVHT) VALUES
('CSDL',  N'Cơ sở dữ liệu', 3),
('JAVA',  N'Lập trình Java', 3),
('CTDL',  N'Cấu trúc dữ liệu và giải thuật', 3),
('WEB',   N'Lập trình Web', 3),
('CNPM',  N'Công nghệ phần mềm', 3),
('HDH',   N'Hệ điều hành', 3),
('MMT',   N'Mạng máy tính', 3),
('OOP',   N'Lập trình hướng đối tượng', 3),
('TA1',   N'Tiếng Anh chuyên ngành 1', 2),
('TA2',   N'Tiếng Anh chuyên ngành 2', 2),
('THDC',  N'Tin học đại cương', 3),
('PTTK',  N'Phân tích và thiết kế hệ thống', 3),
('AI',    N'Trí tuệ nhân tạo', 3),
('LTNC',  N'Lập trình nâng cao', 3),
('KTLT',  N'Kỹ thuật lập trình', 3);
GO

INSERT INTO ChucVu (MaChucVu, TenChucVu) VALUES
('SV', N'Sinh viên'),
('LCT', N'Lớp trưởng'),
('LCP', N'Lớp phó');
GO

INSERT INTO DanToc (MaDanToc, TenDanToc) VALUES
('DT01', N'Kinh'),
('DT02', N'Tày');
GO

INSERT INTO Que (MaQue, TenQue) VALUES
('HN', N'Hà Nội'),
('HP', N'Hải Phòng'),
('NB', N'Ninh Bình'),
('ND', N'Nam Định'),
('TB', N'Thái Bình'),
('LS', N'Lạng Sơn');
GO

INSERT INTO HeDaoTao (MaHeDT, TenHeDT) VALUES
('CQ', N'Chính quy'),
('CLC', N'Chất lượng cao');
GO

INSERT INTO PhongHoc (MaPhong, TenPhong) VALUES
('A101', N'Phòng A101'),
('A102', N'Phòng A102'),
('A103', N'Phòng A103'),
('B201', N'Phòng B201'),
('B202', N'Phòng B202'),
('Lab01', N'Phòng Máy 01'),
('Lab02', N'Phòng Máy 02');
GO

INSERT INTO Khoa_ChuyenNganh (MaChuyenNganh, MaKhoa, TenChuyenNganh) VALUES
('KTPM', 'CNTT', N'Kỹ thuật phần mềm'),
('HTTT', 'CNTT', N'Hệ thống thông tin'),
('KHMT', 'CNTT', N'Khoa học máy tính');
GO

INSERT INTO Lop (MaLop, TenLop, MaKhoa, KhoaHoc, SiSo) VALUES
('CNTT01', N'Công nghệ thông tin 01', 'CNTT','K2025', 9),
('CNTT02', N'Công nghệ thông tin 02', 'CNTT','K2025', 11),
('CNTT03', N'Công nghệ thông tin 03', 'CNTT','K2025', 10),
('CNTT04', N'Công nghệ thông tin 04', 'CNTT','K2026', 10),
('CNTT05', N'Công nghệ thông tin 05', 'CNTT','K2026', 10);
GO

INSERT INTO SinhVien (MaSV, TenSV, MaKhoa, MaLop, NgaySinh, GioiTinh, MaQue, MaDanToc, MaChuyenNganh, MaHeDT, MaChucVu) VALUES
('SV001', N'Nguyễn Văn An', 'CNTT', 'CNTT01', '2005-02-15', N'Nam', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV002', N'Trần Thị Minh Anh', 'CNTT', 'CNTT01', '2005-08-21', N'Nữ', 'HP', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV003', N'Lê Hoàng Nam', 'CNTT', 'CNTT01', '2004-11-02', N'Nam', 'NB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV004', N'Phạm Ngọc Mai', 'CNTT', 'CNTT01', '2005-04-18', N'Nữ', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV005', N'Đỗ Đức Anh', 'CNTT', 'CNTT01', '2005-01-27', N'Nam', 'ND', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV006', N'Hoàng Thị Lan', 'CNTT', 'CNTT01', '2005-07-09', N'Nữ', 'TB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV007', N'Vũ Minh Đức', 'CNTT', 'CNTT01', '2004-12-11', N'Nam', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV008', N'Bùi Thị Hương', 'CNTT', 'CNTT01', '2005-03-25', N'Nữ', 'HP', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV009', N'Đặng Quốc Việt', 'CNTT', 'CNTT01', '2005-09-14', N'Nam', 'NB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV010', N'Nguyễn Thị Ngọc', 'CNTT', 'CNTT01', '2005-06-30', N'Nữ', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV011', N'Trịnh Văn Bình', 'CNTT', 'CNTT02', '2005-02-07', N'Nam', 'ND', 'DT02', 'HTTT', 'CQ', 'SV'),
('SV012', N'Phan Thị Thu', 'CNTT', 'CNTT02', '2005-10-19', N'Nữ', 'TB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV013', N'Nguyễn Đức Long', 'CNTT', 'CNTT02', '2004-05-23', N'Nam', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV014', N'Lý Thị Hoa', 'CNTT', 'CNTT02', '2005-11-05', N'Nữ', 'LS', 'DT02', 'HTTT', 'CQ', 'SV'),
('SV015', N'Trần Quốc Huy', 'CNTT', 'CNTT02', '2005-01-16', N'Nam', 'HP', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV016', N'Phạm Thị Linh', 'CNTT', 'CNTT02', '2005-08-03', N'Nữ', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV017', N'Đinh Văn Khánh', 'CNTT', 'CNTT02', '2004-09-28', N'Nam', 'NB', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV018', N'Mai Thị Phương', 'CNTT', 'CNTT02', '2005-12-20', N'Nữ', 'ND', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV019', N'Ngô Tuấn Kiệt', 'CNTT', 'CNTT02', '2005-04-12', N'Nam', 'TB', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV020', N'Võ Thị Thanh', 'CNTT', 'CNTT02', '2005-07-26', N'Nữ', 'HN', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV021', N'Nguyễn Hoàng Sơn', 'CNTT', 'CNTT03', '2004-03-09', N'Nam', 'HP', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV022', N'Trần Ngọc Hà', 'CNTT', 'CNTT03', '2005-05-17', N'Nữ', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV023', N'Lê Văn Dũng', 'CNTT', 'CNTT03', '2005-10-08', N'Nam', 'ND', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV024', N'Hoàng Thị Yến', 'CNTT', 'CNTT03', '2005-02-28', N'Nữ', 'TB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV025', N'Phạm Minh Quân', 'CNTT', 'CNTT03', '2004-12-03', N'Nam', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV026', N'Đỗ Thị Ngân', 'CNTT', 'CNTT03', '2005-06-14', N'Nữ', 'HP', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV027', N'Vũ Quốc Khải', 'CNTT', 'CNTT03', '2005-09-22', N'Nam', 'NB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV028', N'Bùi Minh Châu', 'CNTT', 'CNTT03', '2005-01-05', N'Nữ', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV029', N'Đặng Văn Tùng', 'CNTT', 'CNTT03', '2004-08-16', N'Nam', 'ND', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV030', N'Nguyễn Thùy Linh', 'CNTT', 'CNTT03', '2005-11-27', N'Nữ', 'TB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV031', N'Phạm Quốc Bảo', 'CNTT', 'CNTT04', '2005-03-18', N'Nam', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV032', N'Trần Thị Hạnh', 'CNTT', 'CNTT04', '2005-07-31', N'Nữ', 'HP', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV033', N'Lê Minh Hoàng', 'CNTT', 'CNTT04', '2004-10-13', N'Nam', 'NB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV034', N'Nguyễn Khánh Ly', 'CNTT', 'CNTT04', '2005-05-06', N'Nữ', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV035', N'Hoàng Văn Thành', 'CNTT', 'CNTT04', '2005-12-09', N'Nam', 'ND', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV036', N'Phan Ngọc Ánh', 'CNTT', 'CNTT04', '2005-02-19', N'Nữ', 'TB', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV037', N'Đỗ Minh Khang', 'CNTT', 'CNTT04', '2004-06-25', N'Nam', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV038', N'Vũ Thị Hằng', 'CNTT', 'CNTT04', '2005-09-07', N'Nữ', 'HP', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV039', N'Bùi Quốc Trung', 'CNTT', 'CNTT04', '2005-01-29', N'Nam', 'ND', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV040', N'Trịnh Thị Mai', 'CNTT', 'CNTT04', '2005-04-04', N'Nữ', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV041', N'Nguyễn Thành Đạt', 'CNTT', 'CNTT05', '2004-11-18', N'Nam', 'TB', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV042', N'Trần Thị Kim Oanh', 'CNTT', 'CNTT05', '2005-08-12', N'Nữ', 'HN', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV043', N'Lê Quốc Tuấn', 'CNTT', 'CNTT05', '2005-03-03', N'Nam', 'HP', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV044', N'Phạm Thị Như Quỳnh', 'CNTT', 'CNTT05', '2005-10-24', N'Nữ', 'ND', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV045', N'Hoàng Minh Tâm', 'CNTT', 'CNTT05', '2004-07-15', N'Nam', 'HN', 'DT01', 'KHMT', 'CQ', 'SV'),
('SV046', N'Nguyễn Thị Vân', 'CNTT', 'CNTT05', '2005-05-29', N'Nữ', 'TB', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV047', N'Đặng Minh Nhật', 'CNTT', 'CNTT05', '2005-09-03', N'Nam', 'NB', 'DT01', 'HTTT', 'CQ', 'SV'),
('SV048', N'Lý Ngọc Trâm', 'CNTT', 'CNTT05', '2005-01-12', N'Nữ', 'LS', 'DT02', 'KHMT', 'CQ', 'SV'),
('SV049', N'Võ Minh Phúc', 'CNTT', 'CNTT05', '2004-12-28', N'Nam', 'HN', 'DT01', 'KTPM', 'CQ', 'SV'),
('SV050', N'Mai Quốc Anh', 'CNTT', 'CNTT05', '2005-06-08', N'Nam', 'HP', 'DT01', 'HTTT', 'CQ', 'SV');
GO

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, PhanQuyen, MaSV, TrangThai) VALUES
('admin', '123456', 1, NULL, 1),
('sv001', '123456', 2, 'SV001', 1),
('sv002', '123456', 2, 'SV002', 1),
('sv003', '123456', 2, 'SV003', 1),
('sv004', '123456', 2, 'SV004', 1),
('sv005', '123456', 2, 'SV005', 1),
('sv006', '123456', 2, 'SV006', 1),
('sv007', '123456', 2, 'SV007', 1),
('sv008', '123456', 2, 'SV008', 1),
('sv009', '123456', 2, 'SV009', 1),
('sv010', '123456', 2, 'SV010', 1),
('sv011', '123456', 2, 'SV011', 1),
('sv012', '123456', 2, 'SV012', 1),
('sv013', '123456', 2, 'SV013', 1),
('sv014', '123456', 2, 'SV014', 1),
('sv015', '123456', 2, 'SV015', 1),
('sv016', '123456', 2, 'SV016', 1),
('sv017', '123456', 2, 'SV017', 1),
('sv018', '123456', 2, 'SV018', 1),
('sv019', '123456', 2, 'SV019', 1),
('sv020', '123456', 2, 'SV020', 1),
('sv021', '123456', 2, 'SV021', 1),
('sv022', '123456', 2, 'SV022', 1),
('sv023', '123456', 2, 'SV023', 1),
('sv024', '123456', 2, 'SV024', 1),
('sv025', '123456', 2, 'SV025', 1),
('sv026', '123456', 2, 'SV026', 1),
('sv027', '123456', 2, 'SV027', 1),
('sv028', '123456', 2, 'SV028', 1),
('sv029', '123456', 2, 'SV029', 1),
('sv030', '123456', 2, 'SV030', 1),
('sv031', '123456', 2, 'SV031', 1),
('sv032', '123456', 2, 'SV032', 1),
('sv033', '123456', 2, 'SV033', 1),
('sv034', '123456', 2, 'SV034', 1),
('sv035', '123456', 2, 'SV035', 1),
('sv036', '123456', 2, 'SV036', 1),
('sv037', '123456', 2, 'SV037', 1),
('sv038', '123456', 2, 'SV038', 1),
('sv039', '123456', 2, 'SV039', 1),
('sv040', '123456', 2, 'SV040', 1),
('sv041', '123456', 2, 'SV041', 1),
('sv042', '123456', 2, 'SV042', 1),
('sv043', '123456', 2, 'SV043', 1),
('sv044', '123456', 2, 'SV044', 1),
('sv045', '123456', 2, 'SV045', 1),
('sv046', '123456', 2, 'SV046', 1),
('sv047', '123456', 2, 'SV047', 1),
('sv048', '123456', 2, 'SV048', 1),
('sv049', '123456', 2, 'SV049', 1),
('sv050', '123456', 2, 'SV050', 1);
GO

INSERT INTO Diem (MaSV, MaLop, MaMon, HocKy, LanThi, Diem) VALUES
('SV001', 'CNTT01', 'CSDL', 1, 1, 8.5),
('SV001', 'CNTT01', 'JAVA', 1, 1, 7.5),
('SV001', 'CNTT01', 'CTDL', 1, 1, 8.0),
('SV001', 'CNTT01', 'WEB', 2, 1, 9.0),
('SV002', 'CNTT01', 'CSDL', 1, 1, 7.0),
('SV002', 'CNTT01', 'JAVA', 1, 1, 8.0);
GO

INSERT INTO ThoiKhoaBieu (MaLop, MaMon, HocKy, ThuHoc, CaHoc, MaPhong) VALUES
('CNTT01','THDC',1,2,1,'A101'),
('CNTT01','KTLT',1,2,3,'A102'),
('CNTT01','CTDL',1,3,1,'Lab01'),
('CNTT01','OOP',1,4,3,'Lab02'),
('CNTT01','CSDL',1,5,1,'A103'),
('CNTT01','JAVA',1,6,3,'Lab01'),
('CNTT01','TA1',1,7,1,'B201'),
('CNTT01','WEB',1,7,3,'Lab02');
GO