using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class TieuChi
    {
        [Key]
        public int TieuChiID { get; set; }
        public int NhomTieuChiID { get; set; }
        public string? TenTieuChi { get; set; }
        public int DiemToiDa { get; set; }
        public bool YeuCauMinhChung { get; set; }

        public NhomTieuChi? NhomTieuChi { get; set; }
    }
}
