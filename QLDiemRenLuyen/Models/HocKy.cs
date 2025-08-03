using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class HocKy
    {
        [Key]
        public int HocKyID { get; set; }

        [Required]
        [MaxLength(50)]
        public string? TenHocKy { get; set; } // VD: "Học kỳ 1"

        [Required]
        [MaxLength(50)]
        public string? NamHoc { get; set; } // VD: "2023-2024"

        [Required]
        public int NienKhoaID { get; set; }

        [ForeignKey("NienKhoaID")]
        public NienKhoa? NienKhoa { get; set; }
    }
}
