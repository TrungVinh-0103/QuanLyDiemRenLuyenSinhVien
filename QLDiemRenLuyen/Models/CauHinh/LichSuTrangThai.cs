using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class LichSuTrangThai
    {
        [Key]
        public int LichSuID { get; set; }
        public int SinhVienID { get; set; }
        public string? TrangThai { get; set; }
        public DateTime? NgayCapNhat { get; set; } = DateTime.Now;
        public string? GhiChu { get; set; }
        public virtual SinhVien? SinhVien { get; set; }
        public LichSuTrangThai()
        {
            NgayCapNhat = DateTime.Now;
        }
    }
}
