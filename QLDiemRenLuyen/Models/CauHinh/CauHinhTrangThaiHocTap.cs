using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class CauHinhTrangThaiHocTap
    {
        [Key]
        public int TrangThaiID { get; set; }
        public string? TenTrangThai { get; set; }
        public string? MoTa { get; set; }
    }

}
