using QLDiemRenLuyen.Models.CauHinh;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class PhieuDanhGia
    {
        [Key]
        public int PhieuDanhGiaID { get; set; }

        [Required]
        public int SinhVienID { get; set; }

        [Required]
        public int HocKyID { get; set; }

        public DateTime NgayLapPhieu { get; set; } = DateTime.Now;

        public int? TrangThaiDanhGiaID { get; set; }

        public int? TongDiemTuDanhGia { get; set; }

        public int? TongDiemGiaoVienDeXuat { get; set; }

        public int? TongDiemHoiDongDuyet { get; set; }

        // ======== Liên kết khóa ngoại ==========

        [ForeignKey("SinhVienID")]
        public SinhVien? SinhVien { get; set; }

        [ForeignKey("HocKyID")]
        public HocKy? HocKy { get; set; }

        [ForeignKey("TrangThaiDanhGiaID")]
        public CauHinhTrangThaiDanhGia? TrangThaiDanhGia { get; set; }

    }
}
