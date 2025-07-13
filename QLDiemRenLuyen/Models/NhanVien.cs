using QLDiemRenLuyen.Models.CauHinh;
using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class NhanVien
    {
        [Key]
        public int NhanVienID { get; set; }
        public string? MaNV { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public int KhoaID { get; set; }
        public string? ChucVu { get; set; }

        public Khoa? Khoa { get; set; }
    }

}
