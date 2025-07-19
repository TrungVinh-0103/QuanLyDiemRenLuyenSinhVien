using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class ThongKe
    {
        [Key]
        public int ThongKeID { get; set; }
        public int HocKyID { get; set; }
        public string? LoaiThongKe { get; set; }
        public string? DieuKienThongKe { get; set; }
        public string? NoiDung { get; set; }
        public DateTime NgayThongKe { get; set; }

        public HocKy? HocKy { get; set; }
    }
}
