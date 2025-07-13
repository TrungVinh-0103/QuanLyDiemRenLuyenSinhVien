using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class Lop
    {
        [Key]
        public int LopID { get; set; }
        public required string TenLop { get; set; }
        public int KhoaID { get; set; }
        public int NienKhoaID { get; set; }

        public Khoa? Khoa { get; set; }
        public NienKhoa? NienKhoa { get; set; }
        //public required ICollection<SinhVien> SinhVien { get; set; }
    }

}
