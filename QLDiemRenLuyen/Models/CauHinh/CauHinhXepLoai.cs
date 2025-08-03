using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class CauHinhXepLoai
    {
        [Key]
        public int XepLoaiID { get; set; }

        [Required(ErrorMessage = "Tên xếp loại không được để trống")]
        [MaxLength(50)]
        public string? TenXepLoai { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm tối thiểu phải >= 0")]
        public int DiemToiThieu { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm tối đa phải >= 0")]
        public int DiemToiDa { get; set; }

        [MaxLength(200)]
        public string? MoTa { get; set; }
    }
}
