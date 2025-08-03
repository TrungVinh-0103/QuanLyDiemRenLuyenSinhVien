using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class NhanVien
    {
        [Key]
        public int NhanVienID { get; set; }

        [Required]
        [StringLength(20)]
        public string? MaNV { get; set; }

        [Required]
        [StringLength(100)]
        public string? HoTen { get; set; }

        public int? KhoaID { get; set; }
        public int ChucVuID { get; set; }

        [ForeignKey("KhoaID")]
        public Khoa? Khoa { get; set; }

        [ForeignKey("ChucVuID")]
        public ChucVu? ChucVu { get; set; }
    }
}
