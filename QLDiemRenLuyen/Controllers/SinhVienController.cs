using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace QLDiemRenLuyen.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang tổng quan sinh viên
        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("Username");
            var vaiTro = HttpContext.Session.GetString("VaiTro");
                
            if (string.IsNullOrEmpty(username) || vaiTro != "SinhVien")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            //var nguoiDung = _context.NguoiDung
            //    .Include(x => x.SinhVien)
            //        .ThenInclude(x => x!.Lop)
            //    .FirstOrDefault(x => x.Username == username && x.VaiTro == vaiTro);
            var nguoiDung = _context.NguoiDung
                .Include(x => x.SinhVien)
                .ThenInclude(x => x!.Lop)
                .ThenInclude(l => l!.Khoa)
                .FirstOrDefault(x => x.Username == username && x.VaiTro == "SinhVien");


            if (nguoiDung == null || nguoiDung.SinhVien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var sv = nguoiDung.SinhVien;
            //
            var ketQuas = _context.KetQuaRenLuyen
                .Include(k => k.HocKy)
                .Where(k => k.SinhVienID == sv.SinhVienID)
                .OrderByDescending(k => k.HocKy!.NgayBatDau)
                .ToList();

            var hocKyMoiNhat = _context.HocKy
                .OrderByDescending(h => h.NgayBatDau)
                .FirstOrDefault();

            //var phieuGanNhat = _context.PhieuDanhGia
            //    .Where(p => p.SinhVienID == sv.SinhVienID && p.HocKyID == hocKyMoiNhat!.HocKyID)
            //    .OrderByDescending(p => p.NgayDanhGia)
            //    .FirstOrDefault();
            var trangThaiMoiNhat = _context.PhieuDanhGia
                .Where(p => p.SinhVienID == sv.SinhVienID && p.HocKyID == hocKyMoiNhat!.HocKyID)
                .Select(p => p.TrangThai)
                .FirstOrDefault() ?? "Chưa đánh giá";

            ViewBag.SinhVien = sv;
            ViewBag.HocKy = hocKyMoiNhat;
            //ViewBag.TrangThai = phieuGanNhat?.TrangThai ?? "Chưa đánh giá";
            ViewBag.TrangThai = trangThaiMoiNhat;
            ViewBag.KetQua = ketQuas;

            return View();
        }

        public IActionResult TuDanhGia()
        {
            var username = HttpContext.Session.GetString("Username");
            var nguoiDung = _context.NguoiDung
            .Include(x => x.SinhVien)
            .FirstOrDefault(x => x.Username == username && x.VaiTro == "SinhVien");

            if (nguoiDung == null || nguoiDung.SinhVien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var hocKyMoiNhat = _context.HocKy.OrderByDescending(h => h.NgayBatDau).FirstOrDefault();
            if (hocKyMoiNhat == null)
            {
                ViewBag.ThongBao = "Chưa có học kỳ nào.";
                return View();
            }

            var nhomTieuChi = _context.NhomTieuChi
                .Include(n => n.TieuChi)
                .Select(n => new NhomTieuChiViewModel
                {
                    NhomTieuChiID = n.NhomTieuChiID,
                    TenNhom = n.TenNhom,
                    DiemToiDa = n.DiemToiDa,
                    TieuChi = n.TieuChi!.Select(t => new TieuChiViewModel
                    {
                        TieuChiID = t.TieuChiID,
                        TenTieuChi = t.TenTieuChi,
                        DiemToiDa = t.DiemToiDa,
                        YeuCauMinhChung = t.YeuCauMinhChung
                    }).ToList()
                }).ToList();

            var model = new TuDanhGiaViewModel
            {
                HocKyID = hocKyMoiNhat.HocKyID,
                NhomTieuChi = nhomTieuChi
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> TuDanhGia(IFormCollection form)
        {
            var username = HttpContext.Session.GetString("Username");
            var nguoiDung = _context.NguoiDung
                .Include(x => x.SinhVien)
                .FirstOrDefault(x => x.Username == username && x.VaiTro == "SinhVien");

            if (nguoiDung == null || nguoiDung.SinhVien == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!form.TryGetValue("HocKyID", out var hocKyIdValue) || string.IsNullOrEmpty(hocKyIdValue))
            {
                TempData["ThongBao"] = "Học kỳ không hợp lệ.";
                return RedirectToAction("TuDanhGia");
            }

            if (!int.TryParse(hocKyIdValue, out var hocKyId))
            {
                TempData["ThongBao"] = "Học kỳ không hợp lệ.";
                return RedirectToAction("TuDanhGia");
            }

            foreach (var key in form.Keys)
            {
                if (key.StartsWith("DiemDanhGia_"))
                {
                    var tieuChiIdStr = key.Replace("DiemDanhGia_", "");
                    if (!int.TryParse(tieuChiIdStr, out int tieuChiId)) continue;

                    var diemStr = form[key];
                    if (!int.TryParse(diemStr, out int diem)) diem = 0;

                    var phieu = new PhieuDanhGia
                    {
                        SinhVienID = nguoiDung.SinhVien.SinhVienID,
                        HocKyID = hocKyId,
                        TieuChiID = tieuChiId,
                        Diem = diem,
                        NguonDanhGia = "SinhVien",
                        TrangThai = "Đã gửi GVCN",
                        NgayDanhGia = DateTime.Now
                    };
                    _context.PhieuDanhGia.Add(phieu);
                    await _context.SaveChangesAsync();

                    var file = form.Files.FirstOrDefault(f => f.Name == $"MinhChung_{tieuChiId}");
                    if (file != null && file.Length > 0)
                    {
                        var folderPath = Path.Combine("wwwroot", "uploads", "minhchung");
                        Directory.CreateDirectory(folderPath);

                        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                        var filePath = Path.Combine(folderPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var minhChung = new MinhChung
                        {
                            PhieuDanhGiaID = phieu.PhieuDanhGiaID,
                            DuongDan = "/uploads/minhchung/" + fileName,
                            MoTa = "Minh chứng tự đánh giá",
                            NgayUpload = DateTime.Now
                        };
                        _context.MinhChung.Add(minhChung);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            TempData["ThongBao"] = "Gửi phiếu tự đánh giá thành công!";
            return RedirectToAction("Index");
        }
        public IActionResult XemKetQua()
        {
            var username = HttpContext.Session.GetString("Username");
            var nguoiDung = _context.NguoiDung
                .Include(x => x.SinhVien)
                    .ThenInclude(sv => sv!.Lop)
                .FirstOrDefault(x => x.Username == username && x.VaiTro == "SinhVien");

            if (nguoiDung == null || nguoiDung.SinhVien == null)
                return RedirectToAction("DangNhap", "TaiKhoan");

            var sv = nguoiDung.SinhVien;

            var ketQua = _context.KetQuaRenLuyen
                .Include(kq => kq.HocKy)
                .Where(kq => kq.SinhVienID == sv.SinhVienID)
                .OrderByDescending(kq => kq.NgayCapNhat)
                .ToList();

            ViewBag.SinhVien = nguoiDung.SinhVien;
            return View(ketQua);
        }

        private string XepLoaiTuDong(int tongDiem)
        {
            var config = _context.CauHinhXepLoai
                      .Where(x => tongDiem >= x.DiemToiThieu && tongDiem <= x.DiemToiDa)
                      .FirstOrDefault();

            return config?.TenXepLoai ?? "Chưa xác định";
        }



    }
}