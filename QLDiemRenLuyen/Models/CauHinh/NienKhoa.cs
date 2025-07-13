using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class NienKhoa
    {
        [Key]
        public int NienKhoaID { get; set; }
        public string? TenNienKhoa { get; set; }
        public int NamBatDau { get; set; }
        public int NamKetThuc { get; set; }

    }

}
