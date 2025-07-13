using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class Khoa
    {
        [Key]
        public int KhoaID { get; set; }
        public string? TenKhoa { get; set; }
        public int TruongID { get; set; }
        public string? DiaChi { get; set; }

        public Truong? Truong { get; set; }
    }

}
