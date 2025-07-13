namespace QLDiemRenLuyen.ViewModels
{
    public class TieuChiViewModel
    {
        public int TieuChiID { get; set; }
        public string? TenTieuChi { get; set; }
        public int DiemToiDa { get; set; }
        public bool YeuCauMinhChung { get; set; }

        public int? DiemSinhVienDanhGia { get; set; }
        public string? FileMinhChungUrl { get; set; }
    }

    public class NhomTieuChiViewModel
    {
        public int NhomTieuChiID { get; set; }
        public string? TenNhom { get; set; }
        public int DiemToiDa { get; set; }

        public List<TieuChiViewModel>? TieuChi { get; set; }
    }

    public class TuDanhGiaViewModel
    {
        public int HocKyID { get; set; }
        public List<NhomTieuChiViewModel>? NhomTieuChi { get; set; }
    }
}
