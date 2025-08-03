using QLDiemRenLuyen.Models.CauHinh;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class KetQuaRenLuyen
    {
        [Key]
        public int KetQuaID { get; set; }

        [Required]
        public int SinhVienID { get; set; }

        [Required]
        public int HocKyID { get; set; }

        [Required]
        public int PhieuDanhGiaID { get; set; }

        [Required]
        public int TongDiemHoiDongDuyet { get; set; }
        
        [Required]
        public int XepLoaiID { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("SinhVienID")]
        public SinhVien? SinhVien { get; set; }

        [ForeignKey("HocKyID")]
        public HocKy? HocKy { get; set; }

        [ForeignKey("PhieuDanhGiaID")]
        public PhieuDanhGia? PhieuDanhGia { get; set; }

        [ForeignKey("XepLoaiID")]
        public CauHinhXepLoai? XepLoai { get; set; }
    }
}
