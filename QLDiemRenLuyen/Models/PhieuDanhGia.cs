using QLDiemRenLuyen.Models.CauHinh;
using System;
using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class PhieuDanhGia
    {
        [Key]
        public int PhieuDanhGiaID { get; set; }
        public int SinhVienID { get; set; }
        public int HocKyID { get; set; }
        public int TieuChiID { get; set; }
        public string? NguonDanhGia { get; set; }
        public int Diem { get; set; }
        public string? TrangThai { get; set; }
        public DateTime NgayDanhGia { get; set; }
        public string? GhiChu { get; set; }

        public SinhVien? SinhVien { get; set; }
        public HocKy? HocKy { get; set; }
        public TieuChi? TieuChi { get; set; }
    }
}