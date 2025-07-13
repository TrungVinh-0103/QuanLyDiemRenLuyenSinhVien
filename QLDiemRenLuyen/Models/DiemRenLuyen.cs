using QLDiemRenLuyen.Models.CauHinh;
using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class DiemRenLuyen
    {
        [Key]
        public int DiemRenLuyenID { get; set; }
        public int SinhVienID { get; set; }
        public int HocKyID { get; set; }
        public int TieuChiID { get; set; }
        public string? NguonDanhGia { get; set; }
        public int Diem { get; set; }
        public int? MinhChungID { get; set; }
        public string? TrangThai { get; set; }

        public SinhVien? SinhVien { get; set; }
        public HocKy? HocKy { get; set; }
        public TieuChi? TieuChi { get; set; }
        public MinhChung? MinhChung { get; set; }
    }
}
