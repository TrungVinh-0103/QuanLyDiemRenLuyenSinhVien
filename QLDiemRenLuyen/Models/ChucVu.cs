using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class ChucVu
    {
        [Key]
        public int ChucVuID { get; set; }

        [Required]
        [StringLength(50)]
        public string? TenChucVu { get; set; }

        public ICollection<NhanVien>? NhanViens { get; set; }
    }
}
