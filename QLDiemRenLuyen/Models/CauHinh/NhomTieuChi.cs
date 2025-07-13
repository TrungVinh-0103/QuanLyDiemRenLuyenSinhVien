using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models.CauHinh
{
    public class NhomTieuChi
    {
        [Key] 
        public int NhomTieuChiID { get; set; }
        public string? TenNhom { get; set; }
        public int DiemToiDa { get; set; }
        public string? MoTa { get; set; }
        public List<TieuChi>? TieuChi { get; set; }
    }
}
