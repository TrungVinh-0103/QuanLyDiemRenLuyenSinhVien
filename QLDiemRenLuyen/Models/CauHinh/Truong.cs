using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class Truong
    {
        [Key]
        public int TruongID { get; set; }
        public string? TenTruong { get; set; }
        public string? DiaChi { get; set; }
        public string? LogoUrl { get; set; }

    }

}
