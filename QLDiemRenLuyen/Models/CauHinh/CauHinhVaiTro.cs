using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class CauHinhVaiTro
    {
        [Key]
        public int VaiTroID { get; set; }

        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        [MaxLength(50)]
        public string? TenVaiTro { get; set; }
    }

}
