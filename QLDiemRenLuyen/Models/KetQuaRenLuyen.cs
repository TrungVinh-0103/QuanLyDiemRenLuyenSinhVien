using QLDiemRenLuyen.Models.CauHinh;
using System;
using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class KetQuaRenLuyen
    {
        [Key]
        public int KetQuaID { get; set; }
        public int SinhVienID { get; set; }
        public int HocKyID { get; set; }
        public int TongDiem { get; set; }
        public string? XepLoai { get; set; }
        public DateTime NgayCapNhat { get; set; }

        public SinhVien? SinhVien { get; set; }
        public HocKy? HocKy { get; set; }
    }
}
