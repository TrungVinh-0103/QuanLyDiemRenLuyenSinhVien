using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.ViewModels;

namespace QLDiemRenLuyen.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var sinhVienID = HttpContext.Session.GetInt32("SinhVienID");
            var sv = _context.SinhVien
                .Include(s => s.Lop).ThenInclude(l => l!.NienKhoa)
                .Include(s => s.Khoa)
                .Include(s => s.TrangThai)
                .FirstOrDefault(s => s.SinhVienID == sinhVienID);

            ViewBag.SinhVien = sv;
            ViewBag.Lop = sv?.Lop;
            return View();
        }

        public IActionResult TuDanhGia(int? hocKyID)
        {
            var sinhVienID = HttpContext.Session.GetInt32("SinhVienID");
            if (sinhVienID == null) return RedirectToAction("Index");

            var sinhVien = _context.SinhVien
                .Include(s => s.TrangThai)
                .Include(s => s.Lop).ThenInclude(l => l!.NienKhoa)
                .FirstOrDefault(s => s.SinhVienID == sinhVienID);

            if (sinhVien == null) return RedirectToAction("Index");

            // ✅ Nếu trạng thái là "Đã nghỉ" hoặc "Bảo lưu" → chặn
            if (sinhVien.TrangThai?.TenTrangThai == "Đã nghỉ" || sinhVien.TrangThai?.TenTrangThai == "Bảo lưu")
            {
                TempData["Loi"] = "Bạn không thể làm bài đánh giá rèn luyện.";
                return RedirectToAction("Index");
            }

            // Lấy danh sách học kỳ phù hợp với sinh viên (theo NienKhoa)
            var hocKys = _context.HocKy
                .Include(h => h.NienKhoa)
                .Where(h => h.NienKhoaID == sinhVien.Lop!.NienKhoaID)
                .OrderByDescending(h => h.HocKyID)
                .ToList();

            ViewBag.HocKys = hocKys;

            var hocKyDuocChon = hocKyID.HasValue ? hocKys.FirstOrDefault(h => h.HocKyID == hocKyID) : hocKys.FirstOrDefault();
            if (hocKyDuocChon == null)
            {
                TempData["Loi"] = "Không tìm thấy học kỳ phù hợp.";
                return RedirectToAction("Index");
            }

            var nhomTieuChis = _context.NhomTieuChi
                .Select(n => new NhomTieuChiViewModel
                {
                    NhomTieuChiID = n.NhomTieuChiID,
                    TenNhom = n.TenNhom,
                    DiemToiDa = n.DiemToiDa,
                    TieuChi = _context.TieuChi
                        .Where(t => t.NhomTieuChiID == n.NhomTieuChiID)
                        .Select(t => new TieuChiViewModel
                        {
                            TieuChiID = t.TieuChiID,
                            TenTieuChi = t.TenTieuChi,
                            DiemToiDa = t.DiemToiDa
                        })
                        .ToList()
                })
                .ToList();

            var model = new TuDanhGiaViewModel
            {
                HocKyID = hocKyDuocChon.HocKyID,
                NhomTieuChi = nhomTieuChis
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TuDanhGia(TuDanhGiaViewModel model)
        {
            var sinhVienID = HttpContext.Session.GetInt32("SinhVienID") ?? 0;
            if (sinhVienID == 0 || model.HocKyID == 0)
            {
                TempData["Loi"] = "Không thể gửi phiếu đánh giá. Vui lòng đăng nhập lại hoặc chọn học kỳ.";
                return RedirectToAction("Index");
            }

            var chiTietPhieuList = new List<ChiTietPhieuDanhGia>();
            var tongTheoNhom = new Dictionary<int, int>();
            var gioiHanTheoNhom = new Dictionary<int, int>();

            foreach (var item in model.DiemTieuChiList)
            {
                var tieuChi = await _context.TieuChi.FindAsync(item.TieuChiID);
                if (tieuChi == null) continue;

                int diemNhap = item.DiemTuDanhGia;

                if (tieuChi.NhomTieuChiID != 0)
                {
                    int nhomID = tieuChi.NhomTieuChiID;

                    if (!tongTheoNhom.ContainsKey(nhomID)) tongTheoNhom[nhomID] = 0;
                    if (!gioiHanTheoNhom.ContainsKey(nhomID))
                    {
                        var nhom = await _context.NhomTieuChi.FindAsync(nhomID);
                        gioiHanTheoNhom[nhomID] = nhom?.DiemToiDa ?? 0;
                    }

                    tongTheoNhom[nhomID] += diemNhap;
                }

                chiTietPhieuList.Add(new ChiTietPhieuDanhGia
                {
                    TieuChiID = item.TieuChiID,
                    DiemTuDanhGia = diemNhap
                });
            }

            // ✅ Tính tổng điểm = tổng từng nhóm (giới hạn theo DiemToiDa nếu vượt)
            int tongDiem = 0;
            foreach (var nhomId in tongTheoNhom.Keys)
            {
                int diemNhom = tongTheoNhom[nhomId];
                int gioiHan = gioiHanTheoNhom[nhomId];
                tongDiem += Math.Min(diemNhom, gioiHan); // ✅ áp giới hạn tại tổng
            }

            // ✅ Tạo và lưu phiếu
            var phieu = new PhieuDanhGia
            {
                SinhVienID = sinhVienID,
                HocKyID = model.HocKyID,
                TrangThaiDanhGiaID = 2,
                NgayLapPhieu = DateTime.Now,
                TongDiemTuDanhGia = tongDiem
            };

            _context.PhieuDanhGia.Add(phieu);
            await _context.SaveChangesAsync();

            // ✅ Gán ID phiếu cho các chi tiết
            foreach (var ct in chiTietPhieuList)
            {
                ct.PhieuDanhGiaID = phieu.PhieuDanhGiaID;
            }

            _context.ChiTietPhieuDanhGia.AddRange(chiTietPhieuList);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Gửi phiếu đánh giá thành công!";
            return RedirectToAction("LichSuDanhGia");
        }

        public IActionResult LichSuDanhGia()
        {
            var sinhVienID = HttpContext.Session.GetInt32("SinhVienID");
            var danhSach = _context.PhieuDanhGia
                .Include(p => p.HocKy)
                .Include(p => p.TrangThaiDanhGia)
                .Where(p => p.SinhVienID == sinhVienID)
                .OrderByDescending(p => p.NgayLapPhieu)
                .ToList();

            return View(danhSach);
        }

        public IActionResult KetQua()
        {
            var sinhVienID = HttpContext.Session.GetInt32("SinhVienID");
            var kq = _context.KetQuaRenLuyen
                .Include(k => k.HocKy)
                .Include(k => k.XepLoai)
                .Where(k => k.SinhVienID == sinhVienID)
                .OrderByDescending(k => k.NgayCapNhat)
                .ToList();
            return View(kq);
        }
    }
}