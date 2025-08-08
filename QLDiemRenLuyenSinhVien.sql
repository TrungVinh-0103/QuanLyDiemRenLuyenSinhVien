-- Tạo cơ sở dữ liệu
CREATE DATABASE QLDiemRenLuyenSinhVien;
GO
USE QLDiemRenLuyenSinhVien;
GO

-- Bảng Cấu hình trạng thái sinh viên (ví dụ: Đang học, Bảo lưu, Thôi học, Tốt nghiệp...)
CREATE TABLE CauHinhTrangThaiSinhVien (
    TrangThaiID INT PRIMARY KEY IDENTITY(1,1),
    TenTrangThai NVARCHAR(50) NOT NULL
);

-- Bảng Cấu hình trạng thái đánh giá (ví dụ: Chưa gửi, Chờ GVCN duyệt, GVCN đã duyệt, Chờ Hội đồng duyệt, Hội đồng đã duyệt, Từ chối (GVCN), Từ chối (Hội đồng))
-- Các trạng thái này sẽ được quản lý trong ứng dụng để điều hướng quy trình duyệt.
CREATE TABLE CauHinhTrangThaiDanhGia (
    TrangThaiDanhGiaID INT PRIMARY KEY IDENTITY(1,1),
    TenTrangThai NVARCHAR(50) NOT NULL
);

-- Bảng Cấu hình vai trò người dùng (ví dụ: SinhVien, GiaoVienChuNhiem, Admin, PhongDaoTao, ThanhVienHoiDong)
CREATE TABLE CauHinhVaiTro (
    VaiTroID INT PRIMARY KEY IDENTITY(1,1),
    TenVaiTro NVARCHAR(50) NOT NULL
);

-- Bảng Cấu hình xếp loại rèn luyện (ví dụ: Xuất sắc, Tốt, Khá, Trung bình, Yếu, Kém)
CREATE TABLE CauHinhXepLoai (
    XepLoaiID INT PRIMARY KEY IDENTITY(1,1),
    TenXepLoai NVARCHAR(50) NOT NULL,
    DiemToiThieu INT NOT NULL,
    DiemToiDa INT NOT NULL,
    MoTa NVARCHAR(200)
);

---

-- Bảng Trường
CREATE TABLE Truong (
    TruongID INT PRIMARY KEY IDENTITY(1,1),
    TenTruong NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(200),
    LogoUrl NVARCHAR(255)
);

-- Bảng Khoa
CREATE TABLE Khoa (
    KhoaID INT PRIMARY KEY IDENTITY(1,1),
    TenKhoa NVARCHAR(100) NOT NULL,
    TruongID INT REFERENCES Truong(TruongID),
    DiaChi NVARCHAR(200)
);

-- Bảng Niên khóa
CREATE TABLE NienKhoa (
    NienKhoaID INT PRIMARY KEY IDENTITY(1,1),
    TenNienKhoa NVARCHAR(50) NOT NULL, -- Ví dụ: "2020-2024"
    NamBatDau INT NOT NULL,
    NamKetThuc INT NOT NULL
);

-- Bảng Lớp
CREATE TABLE Lop (
    LopID INT PRIMARY KEY IDENTITY(1,1),
    TenLop NVARCHAR(50) NOT NULL,
    KhoaID INT REFERENCES Khoa(KhoaID),
    NienKhoaID INT REFERENCES NienKhoa(NienKhoaID)
);


-- Bảng Học kỳ
CREATE TABLE HocKy (
    HocKyID INT PRIMARY KEY IDENTITY(1,1),
    TenHocKy NVARCHAR(50) NOT NULL, -- Ví dụ: "Học kỳ 1", "Học kỳ 2"
    NamHoc NVARCHAR(50) NOT NULL, -- Ví dụ: "2023-2024"
    NienKhoaID INT REFERENCES NienKhoa(NienKhoaID)
);

-- Bảng Sinh viên
CREATE TABLE SinhVien (
    SinhVienID INT PRIMARY KEY IDENTITY(1,1),
    MaSV NVARCHAR(20) UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    NoiSinh NVARCHAR(100) NOT NULL,
    GioiTinh NVARCHAR(10) NOT NULL,
    KhoaID INT REFERENCES Khoa(KhoaID),
    LopID INT REFERENCES Lop(LopID),
    TrangThaiID INT REFERENCES CauHinhTrangThaiSinhVien(TrangThaiID), -- Trạng thái học tập của SV
    NgayCapNhatTrangThai DATE DEFAULT GETDATE()
);

-- Bảng Chức vụ (Mới được thêm vào)
--CREATE TABLE ChucVu (
--    ChucVuID INT PRIMARY KEY IDENTITY(1,1),
--    TenChucVu NVARCHAR(50) NOT NULL UNIQUE,
--);

-- Bảng Nhân viên (đã được sửa đổi để dùng ChucVuID)
CREATE TABLE NhanVien (
    NhanVienID INT PRIMARY KEY IDENTITY(1,1),
    MaNV NVARCHAR(20) UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    KhoaID INT NULL REFERENCES Khoa(KhoaID), -- Có thể NULL nếu là nhân viên toàn trường (Admin, PĐT, Hội đồng cấp trường)
    --ChucVuID INT REFERENCES ChucVu(ChucVuID) -- Thay thế cột ChucVu NVARCHAR bằng khóa ngoại
	VaiTroID INT REFERENCES CauHinhVaiTro(VaiTroID)
);

--Bảng Chủ nhiệm (Mới thêm vào)
CREATE TABLE ChuNhiem (
    ChuNhiemID INT PRIMARY KEY IDENTITY(1,1),
    NhanVienID INT NOT NULL REFERENCES NhanVien(NhanVienID),
    LopID INT NOT NULL REFERENCES Lop(LopID),
    HocKyID INT NOT NULL REFERENCES HocKy(HocKyID),
    GhiChu NVARCHAR(255),
    CONSTRAINT UQ_ChuNhiem UNIQUE (NhanVienID, LopID, HocKyID) -- Mỗi lớp chỉ có 1 GVCN / học kỳ
);

-- Bảng Người dùng (tài khoản đăng nhập)
CREATE TABLE NguoiDung (
    NguoiDungID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    VaiTroID INT REFERENCES CauHinhVaiTro(VaiTroID), -- Liên kết đến bảng cấu hình vai trò
    SinhVienID INT NULL REFERENCES SinhVien(SinhVienID), -- Nếu là tài khoản sinh viên
    NhanVienID INT NULL REFERENCES NhanVien(NhanVienID), -- Nếu là tài khoản nhân viên
    LastLogin DATETIME
);

-- Bảng Nhóm tiêu chí (Ví dụ: Nhận thức tư tưởng chính trị, Đạo đức lối sống...)
CREATE TABLE NhomTieuChi (
    NhomTieuChiID INT PRIMARY KEY IDENTITY(1,1),
    TenNhom NVARCHAR(200) NOT NULL,
    DiemToiDa INT NOT NULL
);

-- Bảng Tiêu chí (Các tiêu chí con trong mỗi nhóm)
CREATE TABLE TieuChi (
    TieuChiID INT PRIMARY KEY IDENTITY(1,1),
    NhomTieuChiID INT REFERENCES NhomTieuChi(NhomTieuChiID),
    TenTieuChi NVARCHAR(600) NOT NULL,
    DiemToiDa INT NOT NULL
);

-- Bảng Phiếu đánh giá (Phiếu tổng thể mà sinh viên gửi đi cho 1 học kỳ)
-- Đây là nơi lưu trạng thái chung của phiếu (chờ duyệt, đã duyệt...)
CREATE TABLE PhieuDanhGia (
    PhieuDanhGiaID INT PRIMARY KEY IDENTITY(1,1),
    SinhVienID INT REFERENCES SinhVien(SinhVienID),
    HocKyID INT REFERENCES HocKy(HocKyID),
    NgayLapPhieu DATE NOT NULL DEFAULT GETDATE(), -- Ngày sinh viên tạo/gửi phiếu
    TrangThaiDanhGiaID INT REFERENCES CauHinhTrangThaiDanhGia(TrangThaiDanhGiaID), -- Trạng thái của phiếu: Chờ GVCN duyệt, GVCN đã duyệt, Chờ Hội đồng duyệt, Hội đồng đã duyệt...
    TongDiemTuDanhGia INT NULL, -- Tổng điểm tự đánh giá của SV
    TongDiemGiaoVienDeXuat INT NULL, -- Tổng điểm sau khi GVCN duyệt (điểm đề xuất lên Hội đồng)
    TongDiemHoiDongDuyet INT NULL, -- Tổng điểm cuối cùng sau khi Hội đồng duyệt
);

-- Bảng Chi tiết phiếu đánh giá (Lưu điểm cụ thể cho từng tiêu chí trong một phiếu)
CREATE TABLE ChiTietPhieuDanhGia (
    ChiTietPhieuDanhGiaID INT PRIMARY KEY IDENTITY(1,1),
    PhieuDanhGiaID INT REFERENCES PhieuDanhGia(PhieuDanhGiaID),
    TieuChiID INT REFERENCES TieuChi(TieuChiID),
    DiemTuDanhGia INT NOT NULL, -- Điểm sinh viên tự đánh giá cho tiêu chí này
    DiemGiaoVienDeXuat INT NULL, -- Điểm giáo viên đề xuất (có thể NULL ban đầu)
    DiemHoiDongDuyet INT NULL, -- Điểm Hội đồng duyệt cho tiêu chí này (điểm cuối cùng)
    -- Ràng buộc kiểm tra điểm không âm.
    CONSTRAINT CK_DiemTuDanhGia_HopLe CHECK (DiemTuDanhGia >= -50 AND DiemTuDanhGia <= 100),
    CONSTRAINT CK_DiemGiaoVienDeXuat_Positive CHECK (DiemTuDanhGia >= -50 AND DiemTuDanhGia <= 100),
    CONSTRAINT CK_DiemHoiDongDuyet_Positive CHECK (DiemTuDanhGia >= -50 AND DiemTuDanhGia <= 100)
    -- Ràng buộc kiểm tra điểm không vượt quá điểm tối đa của tiêu chí đã được loại bỏ.
);

-- Bảng Kết quả rèn luyện (Lưu tổng điểm cuối cùng và xếp loại sau khi đã duyệt bởi Hội đồng)
-- Bảng này dùng để công bố điểm và thống kê
CREATE TABLE KetQuaRenLuyen (
    KetQuaID INT PRIMARY KEY IDENTITY(1,1),
    SinhVienID INT REFERENCES SinhVien(SinhVienID),
    HocKyID INT REFERENCES HocKy(HocKyID),
    PhieuDanhGiaID INT REFERENCES PhieuDanhGia(PhieuDanhGiaID), -- Liên kết đến phiếu đã được duyệt cuối cùng
    TongDiemHoiDongDuyet INT NOT NULL,
    XepLoaiID INT REFERENCES CauHinhXepLoai(XepLoaiID), -- Xếp loại rèn luyện
    NgayCapNhat DATE NOT NULL DEFAULT GETDATE(),
    UNIQUE (SinhVienID, HocKyID) -- Mỗi sinh viên chỉ có 1 kết quả rèn luyện cho 1 học kỳ
);

-- Bảng Lịch sử trạng thái sinh viên (theo dõi thay đổi trạng thái của sinh viên)
CREATE TABLE TrangThaiSinhVien (
    LichSuID INT PRIMARY KEY IDENTITY(1,1),
    SinhVienID INT REFERENCES SinhVien(SinhVienID),
    TrangThaiID INT NULL REFERENCES CauHinhTrangThaiSinhVien(TrangThaiID),
    NgayCapNhat DATETIME DEFAULT GETDATE()
);


select * from CauHinhTrangThaiSinhVien--
select * from CauHinhTrangThaiDanhGia--
select * from CauHinhVaiTro--
select * from CauHinhXepLoai--  
select * from ChiTietPhieuDanhGia --
select * from ChuNhiem--
select * from TieuChi--
select * from Truong --
select * from TrangThaiSinhVien--
select * from Khoa--
select * from KetQuaRenLuyen--
select * from HocKy--
select * from Lop--
select * from NhanVien--
select * from NguoiDung--                                   
select * from NhomTieuChi--
select * from NienKhoa--
select * from PhieuDanhGia--
select * from SinhVien--




-- 1. Thêm dữ liệu mẫu vào các bảng cấu hình (Cần chạy trước để có dữ liệu cho các khóa ngoại)
-- Thêm Trạng thái sinh viên
INSERT INTO CauHinhTrangThaiSinhVien (TenTrangThai) VALUES
(N'Đang học'),
(N'Bảo lưu'),
(N'Đã nghỉ');

-- Thêm Trạng thái đánh giá
INSERT INTO CauHinhTrangThaiDanhGia (TenTrangThai) VALUES
(N'Chưa gửi'),
(N'Chờ GVCN duyệt'),
(N'GVCN đã duyệt'),
(N'Chờ Hội đồng duyệt'),
(N'Hội đồng đã duyệt');

-- Thêm Vai trò người dùng
INSERT INTO CauHinhVaiTro (TenVaiTro) VALUES
(N'Sinh Viên'),
(N'Chủ Nhiệm'),
(N'Hội Đồng'),
(N'Admin');

-- Thêm Xếp loại rèn luyện
INSERT INTO CauHinhXepLoai (TenXepLoai, DiemToiThieu, DiemToiDa, MoTa) VALUES
(N'Xuất sắc', 90, 100, N'Điểm rèn luyện từ 90 đến 100'),
(N'Tốt', 80, 89, N'Điểm rèn luyện từ 80 đến 89'),
(N'Khá', 65, 79, N'Điểm rèn luyện từ 65 đến 79'),
(N'Trung bình', 50, 64, N'Điểm rèn luyện từ 50 đến 64'),
(N'Yếu', 35, 49, N'Điểm rèn luyện từ 35 đến 49'),
(N'Kém', 0, 34, N'Điểm rèn luyện dưới 35');

-- Thêm dữ liệu mẫu cho các bảng khác (để có thể test)
INSERT INTO Truong (TenTruong, DiaChi, LogoUrl) VALUES (N'Trường Đại Học Nam Cần Thơ', N'168 Nguyễn Văn Cừ, Cần Thơ', N'https://nctu.edu.vn/webp/logo_truong.webp');
INSERT INTO Khoa (TenKhoa, TruongID) VALUES (N'Công nghệ thông tin', 1);
INSERT INTO NienKhoa (TenNienKhoa, NamBatDau, NamKetThuc) VALUES (N'2022-2026', 2022, 2026);
INSERT INTO Lop (TenLop, KhoaID, NienKhoaID) VALUES (N'DH22TIN02', 1, 1);
INSERT INTO HocKy (TenHocKy, NamHoc, NienKhoaID) VALUES (N'Học kỳ 1', N'2024-2025', 1);

INSERT INTO SinhVien (MaSV, HoTen, NgaySinh, NoiSinh, GioiTinh, KhoaID, LopID, TrangThaiID) VALUES
(N'221576', N'Đinh Trung Vĩnh', '2004-03-01', N'Cần Thơ', N'Nam', 1, 1, 1),
(N'222918', N'Từ Công Vinh', '2004-07-26', N'An Giang', N'Nam', 1, 1, 1);

-- Thêm dữ liệu vào bảng ChucVu
--INSERT INTO ChucVu (TenChucVu) VALUES
--(N'Chủ nhiệm'),
--(N'Hội đồng'),
--(N'Admin');

-- Thêm dữ liệu vào bảng NhanVien (sử dụng ChucVuID)
DECLARE @gvcnVaiTroID INT = (SELECT VaiTroID FROM CauHinhVaiTro WHERE TenVaiTro = N'Chủ nhiệm');
INSERT INTO NhanVien (MaNV, HoTen, KhoaID, VaiTroID) VALUES
(N'GV001', N'Lê Thị C', 1, @gvcnVaiTroID);

DECLARE @hdVaiTroID INT = (SELECT ChucVuID FROM ChucVu WHERE TenVaiTro = N'Hội đồng');
INSERT INTO NhanVien (MaNV, HoTen, KhoaID, VaiTroID) VALUES
(N'HD001', N'Phạm Văn D', 1, @hdVaiTroID); -- Hội đồng có thể không thuộc khoa cụ thể

INSERT INTO NguoiDung (Username, PasswordHash, VaiTroID, SinhVienID, NhanVienID) VALUES
(N'Admin', N'1234', 4, NULL, NULL),
(N'221576', N'1111', 1, 1, NULL),
(N'GV001', N'1111', 2, NULL, 1),
(N'HD001', N'1111', 3, NULL, 2);




