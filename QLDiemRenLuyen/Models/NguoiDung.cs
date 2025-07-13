using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class NguoiDung
    {
        [Key]
        public int NguoiDungID { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? VaiTro { get; set; }
        public DateTime? LastLogin { get; set; }

        public int? SinhVienID { get; set; }
        public SinhVien? SinhVien { get; set; }

        public int? NhanVienID { get; set; }
        public NhanVien? NhanVien { get; set; }
    }

}
