using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class Khoa
    {
        [Key]
        public int KhoaID { get; set; }

        [Required(ErrorMessage = "Tên khoa không được để trống")]
        [MaxLength(100)]
        public string? TenKhoa { get; set; }

        public int TruongID { get; set; }

        [ForeignKey("TruongID")]
        public Truong? Truong { get; set; }

        [MaxLength(200)]
        public string? DiaChi { get; set; }
    }
}
