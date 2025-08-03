using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class ChiTietPhieuDanhGia
    {
        [Key]
        public int ChiTietPhieuDanhGiaID { get; set; }

        [Required]
        public int PhieuDanhGiaID { get; set; }

        [Required]
        public int TieuChiID { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm phải là số không âm.")]
        public int DiemTuDanhGia { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Điểm phải là số không âm.")]
        public int? DiemGiaoVienDeXuat { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Điểm phải là số không âm.")]
        public int? DiemHoiDongDuyet { get; set; }


        [ForeignKey("PhieuDanhGiaID")]
        public PhieuDanhGia? PhieuDanhGia { get; set; }

        [ForeignKey("TieuChiID")]
        public TieuChi? TieuChi { get; set; }
    }
}
