using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QLDiemRenLuyen.Models.CauHinh;

namespace QLDiemRenLuyen.Models
{
    public class NguoiDung
    {
        [Key]
        public int NguoiDungID { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [StringLength(255)]
        public string? PasswordHash { get; set; }

        [ForeignKey("VaiTro")]
        public int VaiTroID { get; set; }

        public int? SinhVienID { get; set; }

        public int? NhanVienID { get; set; }

        public DateTime? LastLogin { get; set; }

        public SinhVien? SinhVien { get; set; }

        public NhanVien? NhanVien { get; set; }

        public CauHinhVaiTro? VaiTro { get; set; }
    }
}
