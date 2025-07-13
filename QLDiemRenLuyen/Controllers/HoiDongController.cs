using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using System.Collections.Generic;

namespace QLDiemRenLuyen.Controllers
{
    public class HoiDongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoiDongController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Danh sách phiếu đánh giá đang chờ hội đồng
            var dsPhieu = _context.PhieuDanhGia
            .Include(d => d.SinhVien)
                .ThenInclude(sv => sv!.Lop)
            .Include(d => d.TieuChi)
                .ThenInclude(tc => tc!.NhomTieuChi)
            .Include(d => d.HocKy)
            .Where(d => d.NguonDanhGia == "GVCN" && d.TrangThai == "Chờ hội đồng")
            .ToList();
            
            ViewBag.PhieuChoCham = dsPhieu;

            return View();
        }

        public IActionResult XemPhieu(int svId, int hocKyId)
        {
            var sv = _context.SinhVien
                .Include(x => x.Lop)
                .FirstOrDefault(x => x.SinhVienID == svId);

            var diemGVCN = _context.PhieuDanhGia
                .Include(d => d.TieuChi)
                    .ThenInclude(tc => tc!.NhomTieuChi)
                .Where(d => d.SinhVienID == svId && d.HocKyID == hocKyId && d.NguonDanhGia == "GVCN")
                .ToList();

            ViewBag.SinhVien = sv;
            ViewBag.HocKy = _context.HocKy.Find(hocKyId);

            return View(diemGVCN);
        }
        [HttpPost]
        public IActionResult XacNhanHoiDong(int SinhVienID, int HocKyID, int[] TieuChiIDs, int[] DiemHoiDongs, string GhiChu)
        {
            // 1. Ghi điểm hội đồng vào bảng DiemRenLuyen (Nguon = HoiDong)
            for (int i = 0; i < TieuChiIDs.Length; i++)
            {
                var diem = new DiemRenLuyen
                {
                    SinhVienID = SinhVienID,
                    HocKyID = HocKyID,
                    TieuChiID = TieuChiIDs[i],
                    Diem = DiemHoiDongs[i],
                    NguonDanhGia = "HoiDong",
                    TrangThai = "Hội đồng đã duyệt"
                };
                _context.DiemRenLuyen.Add(diem);
            }

            // 2. Cập nhật trạng thái phiếu đánh giá SV
            var phieu = _context.PhieuDanhGia
                .Where(p => p.SinhVienID == SinhVienID && p.HocKyID == HocKyID)
                .ToList();

            foreach (var p in phieu)
            {
                p.TrangThai = "Hội đồng đã duyệt";
                if (!string.IsNullOrEmpty(GhiChu))
                    p.GhiChu = GhiChu;
            }

            // 3. Tính tổng điểm hội đồng
            int tongDiem = DiemHoiDongs.Sum();

            // 4. Xác định xếp loại
            var xepLoai = _context.CauHinhXepLoai
                .FirstOrDefault(x => tongDiem >= x.DiemToiThieu && tongDiem <= x.DiemToiDa);

            var ketQua = new KetQuaRenLuyen
            {
                SinhVienID = SinhVienID,
                HocKyID = HocKyID,
                TongDiem = tongDiem,
                XepLoai = xepLoai?.TenXepLoai ?? "Chưa xác định",
                NgayCapNhat = DateTime.Now
            };
            _context.KetQuaRenLuyen.Add(ketQua);

            _context.SaveChanges();
            TempData["ThongBao"] = "Đã xác nhận điểm chính thức và lưu kết quả.";

            return RedirectToAction("Index");
        }

    }
}
