using Microsoft.AspNetCore.Http;
using QLDiemRenLuyen.Models;

namespace QLDiemRenLuyen.ViewModels
{
    public class TieuChiViewModel
    {
        public int TieuChiID { get; set; }
        public string? TenTieuChi { get; set; }
        public int DiemToiDa { get; set; }
        public int DiemTuDanhGia { get; set; } 
        public int? DiemSinhVienDanhGia { get; set; }

        public int? NhomTieuChiID { get; set; }

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
        public int PhieuDanhGiaID { get; set; }
        public int HocKyID { get; set; }
        public int SinhVienID { get; set; }
        public int TongDiemTuDanhGia { get; set; }
        public int TongDiemGiaoVienDeXuat { get; set; }
        public int TongDiemHoiDongDuyet { get; set; }

        //public List<HocKyViewModel>? HocKy { get; set; }
        public List<NhomTieuChiViewModel>? NhomTieuChi { get; set; } = new();
        public List<TieuChiViewModel>? TieuChi { get; set; } = new();
        public List<ChiTietPhieuDanhGia>? ChiTietPhieu { get; set; } = new();
        public List<DiemTieuChiInput> DiemTieuChiList { get; set; } = new();

    }

    //public class HocKyViewModel
    //{
    //    public int HocKyID { get; set; }
    //    public string? TenHocKy { get; set; }
    //}

    public class DiemTieuChiInput
    {
        public int TieuChiID { get; set; }
        public int DiemTuDanhGia { get; set; }
    }

}