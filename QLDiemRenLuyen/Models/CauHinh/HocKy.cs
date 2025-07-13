using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class HocKy
    {
        [Key]
        public int HocKyID { get; set; }
        public string? TenHocKy { get; set; }
        public string? NamHoc { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

    }

}
