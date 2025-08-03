using QLDiemRenLuyen.Models.CauHinh;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class SinhVien
    {
        [Key]
        public int SinhVienID { get; set; }

        [Required]
        [StringLength(20)]
        public string MaSV { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Required]
        public string NoiSinh { get; set; } = string.Empty;

        [Required]
        public string GioiTinh { get; set; } = string.Empty;

        public int KhoaID { get; set; }
        public int LopID { get; set; }
        public int TrangThaiID { get; set; }

        public DateTime? NgayCapNhatTrangThai { get; set; }

        [ForeignKey("KhoaID")]
        public Khoa? Khoa { get; set; }

        [ForeignKey("LopID")]
        public Lop? Lop { get; set; }

        [ForeignKey("TrangThaiID")]
        public CauHinhTrangThaiSinhVien? TrangThai { get; set; }
    }
}
