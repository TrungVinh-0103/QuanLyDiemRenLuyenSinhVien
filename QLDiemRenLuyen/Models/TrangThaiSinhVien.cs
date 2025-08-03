using QLDiemRenLuyen.Models.CauHinh;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class TrangThaiSinhVien
    {
        [Key]
        public int LichSuID { get; set; }

        [Required]
        public int SinhVienID { get; set; }

        public int TrangThaiID { get; set; }

        public DateTime NgayCapNhat { get; set; } = DateTime.Now;

        [ForeignKey("SinhVienID")]
        public SinhVien? SinhVien { get; set; }

        [ForeignKey("TrangThaiID")]
        public CauHinhTrangThaiSinhVien? TrangThai { get; set; }


    }
}
