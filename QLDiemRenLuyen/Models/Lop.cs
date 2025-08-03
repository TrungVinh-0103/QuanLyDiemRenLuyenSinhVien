using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDiemRenLuyen.Models
{
    public class Lop
    {
        [Key]
        public int LopID { get; set; }

        [Required]
        [MaxLength(50)]
        public string? TenLop { get; set; }

        [Required]
        public int KhoaID { get; set; }

        [ForeignKey("KhoaID")]
        public Khoa? Khoa { get; set; }

        [Required]
        public int NienKhoaID { get; set; }

        [ForeignKey("NienKhoaID")]
        public NienKhoa? NienKhoa { get; set; }
    }
}
