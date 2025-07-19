using QLDiemRenLuyen.Models.CauHinh;
using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class SinhVien
    {
        [Key]
        public int SinhVienID { get; set; }
        public string? MaSV { get; set; }
        public string? HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int LopID { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayCapNhatTrangThai { get; set; } = DateTime.Now;
        public string? Email { get; set; }

        public Lop? Lop { get; set; }
        public ICollection<KetQuaRenLuyen>? KetQuaRenLuyen { get; set; }


    }

}
