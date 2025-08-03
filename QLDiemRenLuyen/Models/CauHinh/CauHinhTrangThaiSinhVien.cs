using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class CauHinhTrangThaiSinhVien
    {
        [Key]
        public int TrangThaiID { get; set; }

        [Required(ErrorMessage = "Tên trạng thái không được để trống")]
        [MaxLength(50)]
        public string? TenTrangThai { get; set; }
    }
}
