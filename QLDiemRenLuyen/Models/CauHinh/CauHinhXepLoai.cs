using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class CauHinhXepLoai
    {
        [Key]
        public int XepLoaiID { get; set; }
        public string? TenXepLoai { get; set; }
        public int DiemToiThieu { get; set; }
        public int DiemToiDa { get; set; }
        public string? MoTa { get; set; }
    }

}
