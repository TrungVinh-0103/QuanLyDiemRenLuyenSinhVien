using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class CauHinhVaiTro
    {
        [Key]
        public int VaiTroID { get; set; }
        public string? TenVaiTro { get; set; }
        public string? MoTa { get; set; }
    }

}
