using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class NienKhoa
    {
        [Key]
        public int NienKhoaID { get; set; }

        [Required(ErrorMessage = "Tên niên khóa không được để trống")]
        [MaxLength(50)]
        public string? TenNienKhoa { get; set; } // Ví dụ: "2022-2026"

        [Required]
        public int NamBatDau { get; set; }

        [Required]
        public int NamKetThuc { get; set; }
    }
}
