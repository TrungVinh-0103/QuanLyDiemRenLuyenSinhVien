using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.Models.CauHinh;
using System.Linq;

namespace QLDiemRenLuyen.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult QuanLyNguoiDung()
        {
            var dsNguoiDung = _context.NguoiDung
                .Select(x => new
                {
                    x.NguoiDungID,
                    x.Username,
                    x.VaiTro,
                    x.LastLogin
                })
                .ToList();

            ViewBag.NguoiDungs = dsNguoiDung;
            return View();
        }

        [HttpPost]
        public IActionResult ThemNguoiDung(string username, string password, string vaiTro)
        {
            var user = new NguoiDung
            {
                Username = username,
                PasswordHash = password,
                VaiTro = vaiTro
            };
            _context.NguoiDung.Add(user);
            _context.SaveChanges();
            return RedirectToAction("QuanLyNguoiDung");
        }

        public IActionResult XoaNguoiDung(int id)
        {
            var user = _context.NguoiDung.Find(id);
            if (user != null)
            {
                _context.NguoiDung.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyNguoiDung");
        }

        public IActionResult ResetMatKhau(int id)
        {
            var user = _context.NguoiDung.Find(id);
            if (user != null)
            {
                user.PasswordHash = user.VaiTro == "SinhVien" ? "1111" : "0000";
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyNguoiDung");
        }

        public IActionResult CauHinhHeThong()
        {
            ViewBag.Truong = _context.Truong.ToList();
            ViewBag.Khoa = _context.Khoa.Include(x => x.Truong).ToList();
            ViewBag.Lop = _context.Lop.Include(x => x.Khoa).Include(x => x.NienKhoa).ToList();
            ViewBag.NienKhoa = _context.NienKhoa.ToList();
            ViewBag.HocKy = _context.HocKy.ToList();
            ViewBag.TrangThaiHocTap = _context.CauHinhTrangThaiHocTap.ToList();
            ViewBag.VaiTro = _context.CauHinhVaiTro.ToList();
            ViewBag.XepLoai = _context.CauHinhXepLoai.ToList();
            ViewBag.NhomTieuChi = _context.NhomTieuChi.ToList();
            ViewBag.TieuChi = _context.TieuChi.Include(t => t.NhomTieuChi).ToList();
            ViewBag.MinhChung = _context.MinhChung.ToList();
            ViewBag.PhieuDanhGia = _context.PhieuDanhGia.ToList();
            ViewBag.DiemRenLuyen = _context.DiemRenLuyen.ToList();
            ViewBag.KetQuaRenLuyen = _context.KetQuaRenLuyen.ToList();

            return View();
        }

        // Controllers/AdminController.cs - Phần mở rộng

        [HttpPost]
        public IActionResult ThemTruong(string TenTruong, string DiaChi, string LogoUrl)
        {
            var truong = new Truong { TenTruong = TenTruong, DiaChi = DiaChi, LogoUrl = LogoUrl };
            _context.Truong.Add(truong);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        [HttpPost]
        public IActionResult ThemKhoa(string TenKhoa, int TruongID, string DiaChi)
        {
            var khoa = new Khoa { TenKhoa = TenKhoa, TruongID = TruongID, DiaChi = DiaChi };
            _context.Khoa.Add(khoa);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        [HttpPost]
        public IActionResult ThemLop(string TenLop, int KhoaID, int NienKhoaID)
        {
            var lop = new Lop { TenLop = TenLop, KhoaID = KhoaID, NienKhoaID = NienKhoaID };
            _context.Lop.Add(lop);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        [HttpPost]
        public IActionResult ThemNienKhoa(string TenNienKhoa, int NamBatDau, int NamKetThuc)
        {
            var nk = new NienKhoa { TenNienKhoa = TenNienKhoa, NamBatDau = NamBatDau, NamKetThuc = NamKetThuc };
            _context.NienKhoa.Add(nk);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        [HttpPost]
        public IActionResult ThemHocKy(string TenHocKy, string NamHoc, DateTime? NgayBatDau, DateTime? NgayKetThuc)
        {
            var hk = new HocKy
            {
                TenHocKy = TenHocKy,
                NamHoc = NamHoc,
                NgayBatDau = NgayBatDau,
                NgayKetThuc = NgayKetThuc
            };
            _context.HocKy.Add(hk);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong", new { tab = "hocky" });
        }


        [HttpPost]
        public IActionResult ThemTrangThaiHocTap(string TenTrangThai, string MoTa)
        {
            var tt = new CauHinhTrangThaiHocTap { TenTrangThai = TenTrangThai, MoTa = MoTa };
            _context.CauHinhTrangThaiHocTap.Add(tt);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        [HttpPost]
        public IActionResult ThemVaiTro(string TenVaiTro, string MoTa)
        {
            var vt = new CauHinhVaiTro { TenVaiTro = TenVaiTro, MoTa = MoTa };
            _context.CauHinhVaiTro.Add(vt);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        [HttpPost]
        public IActionResult ThemXepLoai(string TenXepLoai, int DiemToiThieu, int DiemToiDa, string MoTa)
        {
            var xl = new CauHinhXepLoai
            {
                TenXepLoai = TenXepLoai,
                DiemToiThieu = DiemToiThieu,
                DiemToiDa = DiemToiDa,
                MoTa = MoTa
            };
            _context.CauHinhXepLoai.Add(xl);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        // Controllers/AdminController.cs – Thêm phần quản lý nhóm tiêu chí và tiêu chí

        [HttpPost]
        public IActionResult ThemNhomTieuChi(string TenNhom, int DiemToiDa, string MoTa)
        {
            var nhom = new NhomTieuChi { TenNhom = TenNhom, DiemToiDa = DiemToiDa, MoTa = MoTa };
            _context.NhomTieuChi.Add(nhom);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }

        [HttpPost]
        public IActionResult ThemTieuChi(int NhomTieuChiID, string TenTieuChi, int DiemToiDa, bool YeuCauMinhChung)
        {
            var nhomTieuChi = _context.NhomTieuChi.Find(NhomTieuChiID);
            if (nhomTieuChi == null)
            {
                // Handle the case where the NhomTieuChiID is invalid
                return BadRequest("NhomTieuChiID không hợp lệ.");
            }

            var tc = new TieuChi
            {
                NhomTieuChiID = NhomTieuChiID,
                TenTieuChi = TenTieuChi,
                DiemToiDa = DiemToiDa,
                YeuCauMinhChung = YeuCauMinhChung,
                NhomTieuChi = nhomTieuChi // Set the required member explicitly
            };

            _context.TieuChi.Add(tc);
            _context.SaveChanges();
            return RedirectToAction("CauHinhHeThong");
        }
    }
}
