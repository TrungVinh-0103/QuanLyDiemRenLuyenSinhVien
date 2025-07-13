using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;

namespace QLDiemRenLuyen.Controllers
{
    public class GVCNController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GVCNController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");

            var nguoiDung = _context.NguoiDung
                .Include(x => x.NhanVien)
                .ThenInclude(x => x!.Khoa)
                .FirstOrDefault(x => x.Username == username && x.VaiTro == "GVCN");

            if (nguoiDung == null || nguoiDung.NhanVien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var nhanVien = nguoiDung.NhanVien;

            // Danh sách lớp đang chủ nhiệm 
            var danhSachLop = _context.Lop
                .Include(l => l.NienKhoa)
                .Include(l => l.Khoa)
                .Where(l => l.KhoaID == nhanVien.KhoaID)
                .ToList();

            ViewBag.NhanVien = nhanVien;
            return View(danhSachLop);
        }
        public IActionResult XemSinhVien(int lopId)
        {
            var lop = _context.Lop
                .Include(l => l.Khoa)
                .Include(l => l.NienKhoa)
                .FirstOrDefault(l => l.LopID == lopId);

            if (lop == null) return NotFound();

            var danhSachSV = _context.SinhVien
                .Where(sv => sv.LopID == lopId)
                .OrderBy(sv => sv.HoTen)
                .ToList();

            ViewBag.Lop = lop;
            return View(danhSachSV);
        }
        public IActionResult XemPhieuDanhGia(int svId)
        {
            var sv = _context.SinhVien
                .Include(s => s.Lop)
                    .ThenInclude(l => l.Khoa)
                .FirstOrDefault(s => s.SinhVienID == svId);

            if (sv == null) return NotFound();

            var hocKy = _context.HocKy
                .OrderByDescending(h => h.NgayBatDau)
                .FirstOrDefault();

            var danhGia = _context.PhieuDanhGia
                .Include(p => p.TieuChi)
                    .ThenInclude(tc => tc!.NhomTieuChi)
                .Where(p => p.SinhVienID == svId && p.HocKyID == hocKy!.HocKyID && p.NguonDanhGia == "SinhVien")
                .ToList();

            ViewBag.SinhVien = sv;
            ViewBag.HocKy = hocKy;
            return View(danhGia);
        }
        [HttpPost]
        public IActionResult DuyetPhieuDanhGia(int SinhVienID, int HocKyID, int[] TieuChiIDs, int[] DiemGVCNs, string GhiChu)
        {
            for (int i = 0; i < TieuChiIDs.Length; i++)
            {
                int tcID = TieuChiIDs[i];
                int diem = DiemGVCNs[i];

                var diemCu = _context.PhieuDanhGia.FirstOrDefault(d =>
                    d.SinhVienID == SinhVienID &&
                    d.HocKyID == HocKyID &&
                    d.TieuChiID == tcID &&
                    d.NguonDanhGia == "GVCN");

                if (diemCu != null)
                {
                    diemCu.Diem = diem;
                    diemCu.TrangThai = "Chờ hội đồng";
                }
                else
                {
                    var phieuMoi = new PhieuDanhGia
                    {
                        SinhVienID = SinhVienID,
                        HocKyID = HocKyID,
                        TieuChiID = tcID,
                        Diem = diem,
                        NguonDanhGia = "GVCN",
                        TrangThai = "Chờ hội đồng"
                    };
                    _context.PhieuDanhGia.Add(phieuMoi);
                }
            }

            // Cập nhật trạng thái phiếu đánh giá của sinh viên
            var phieuSV = _context.PhieuDanhGia
                .Where(p => p.SinhVienID == SinhVienID && p.HocKyID == HocKyID)
                .ToList();

            foreach (var phieu in phieuSV)
            {
                phieu.TrangThai = "Chờ hội đồng";
                if (!string.IsNullOrEmpty(GhiChu))
                {
                    phieu.GhiChu = GhiChu;
                }
            }

            _context.SaveChanges();
            TempData["ThongBao"] = "Đã duyệt và gửi phiếu cho hội đồng.";

            return RedirectToAction("XemSinhVien", new { lopID = _context.SinhVien.Find(SinhVienID)?.LopID });
        }

    }

}
