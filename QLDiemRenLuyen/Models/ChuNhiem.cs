using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class ChuNhiem
    {
        [Key] 
        public int ChuNhiemID { get; set; }

        [Required]
        public int NhanVienID { get; set; }

        [Required]
        public int LopID { get; set; } 

        [Required]
        public int HocKyID { get; set; }

        [Required]
        [MaxLength(500)]
        public string? GhiChu { get; set; } = "Không có";

        public NhanVien? NhanVien { get; set; }
        public Lop? Lop { get; set; }
        public HocKy? HocKy { get; set; }

        //-- Mỗi lớp chỉ có 1 GVCN / học kỳ
        public bool IsChuNhiem()
        {
            return NhanVienID > 0 && LopID > 0 && HocKyID > 0;
        }

    }
}
