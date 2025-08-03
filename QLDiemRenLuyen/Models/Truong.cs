using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class Truong
    {
        [Key]
        public int TruongID { get; set; }

        [Required(ErrorMessage = "Tên trường không được để trống")]
        [MaxLength(100)]
        public string? TenTruong { get; set; }

        [MaxLength(200)]
        public string? DiaChi { get; set; }

        [MaxLength(255)]
        public string? LogoUrl { get; set; }
    }
}
