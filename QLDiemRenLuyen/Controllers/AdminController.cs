using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.Models.CauHinh;
using BCrypt.Net;

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
            var thongKe = new
            {
                SoSinhVien = _context.SinhVien.Count(),
                SoNhanVien = _context.NhanVien.Count(),
                SoLop = _context.Lop.Count(),
                SoKhoa = _context.Khoa.Count(),
                SoNguoiDung = _context.NguoiDung.Count(),
                SoPhieu = _context.PhieuDanhGia.Count()
            };

            ViewBag.ThongKe = thongKe;
            return View();
        }

        //Cấu hình Trạng thái Sinh viên
        //=======================================================================================
        #region Cấu hình Trạng thái Sinh viên
        [HttpGet]
        public async Task<IActionResult> QuanLyCauHinhTrangThaiSinhVien()
        {
            var list = await _context.CauHinhTrangThaiSinhVien.ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ThemTrangThaiSinhVien(CauHinhTrangThaiSinhVien trangThai)
        {
            if (_context.CauHinhTrangThaiSinhVien.Any(x => x.TenTrangThai == trangThai.TenTrangThai))
            {
                TempData["Loi"] = "Tên trạng thái đã tồn tại!";
            }
            else
            {
                _context.Add(trangThai);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Đã thêm trạng thái thành công!";
            }
            return RedirectToAction("QuanLyCauHinhTrangThaiSinhVien");
        }

        [HttpPost]
        public async Task<IActionResult> SuaTrangThaiSinhVien(CauHinhTrangThaiSinhVien trangThai)
        {
            if (_context.CauHinhTrangThaiSinhVien.Any(x => x.TenTrangThai == trangThai.TenTrangThai && x.TrangThaiID != trangThai.TrangThaiID))
            {
                TempData["Loi"] = "Tên trạng thái đã tồn tại!";
            }
            else
            {
                _context.Update(trangThai);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật thành công!";
            }
            return RedirectToAction("QuanLyCauHinhTrangThaiSinhVien");
        }

        [HttpPost]
        public async Task<IActionResult> XoaTrangThaiSinhVien(int id)
        {
            var item = await _context.CauHinhTrangThaiSinhVien.FindAsync(id);
            if (item == null)
            {
                TempData["Loi"] = "Không tìm thấy trạng thái cần xóa.";
            }
            else if (_context.SinhVien.Any(sv => sv.TrangThaiID == id))
            {
                TempData["Loi"] = "Không thể xóa. Trạng thái này đang được sử dụng bởi sinh viên!";
            }
            else
            {
                _context.CauHinhTrangThaiSinhVien.Remove(item);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa trạng thái thành công!";
            }
            return RedirectToAction("QuanLyCauHinhTrangThaiSinhVien");
        }
        #endregion
        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Cấu hình Trạng thái Đánh giá
        //=======================================================================================
        #region Cấu hình Trạng thái Đánh giá
        [HttpGet]
        public async Task<IActionResult> QuanLyCauHinhTrangThaiDanhGia()
        {
            var list = await _context.CauHinhTrangThaiDanhGia.ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ThemTrangThaiDanhGia(CauHinhTrangThaiDanhGia model)
        {
            if (_context.CauHinhTrangThaiDanhGia.Any(x => x.TenTrangThai == model.TenTrangThai))
            {
                TempData["Loi"] = "Tên trạng thái đánh giá đã tồn tại!";
            }
            else
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Đã thêm trạng thái đánh giá thành công!";
            }
            return RedirectToAction("QuanLyCauHinhTrangThaiDanhGia");
        }

        [HttpPost]
        public async Task<IActionResult> SuaTrangThaiDanhGia(CauHinhTrangThaiDanhGia model)
        {
            if (_context.CauHinhTrangThaiDanhGia.Any(x => x.TenTrangThai == model.TenTrangThai && x.TrangThaiDanhGiaID != model.TrangThaiDanhGiaID))
            {
                TempData["Loi"] = "Tên trạng thái đánh giá đã tồn tại!";
            }
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật trạng thái đánh giá thành công!";
            }
            return RedirectToAction("QuanLyCauHinhTrangThaiDanhGia");
        }

        [HttpPost]
        public async Task<IActionResult> XoaTrangThaiDanhGia(int id)
        {
            var item = await _context.CauHinhTrangThaiDanhGia.FindAsync(id);
            if (item == null)
            {
                TempData["Loi"] = "Không tìm thấy trạng thái cần xóa.";
            }
            else if (_context.PhieuDanhGia.Any(p => p.TrangThaiDanhGiaID == id))
            {
                TempData["Loi"] = "Không thể xóa. Trạng thái này đang được sử dụng bởi phiếu đánh giá!";
            }
            else
            {
                _context.CauHinhTrangThaiDanhGia.Remove(item);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa trạng thái đánh giá thành công!";
            }
            return RedirectToAction("QuanLyCauHinhTrangThaiDanhGia");
        }
        #endregion

        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Cấu hình Vai trò
        //=======================================================================================
        #region Cấu hình Vai trò người dùng
        [HttpGet]
        public async Task<IActionResult> QuanLyCauHinhVaiTro()
        {
            var list = await _context.CauHinhVaiTro.ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ThemVaiTro(CauHinhVaiTro model)
        {
            if (_context.CauHinhVaiTro.Any(x => x.TenVaiTro == model.TenVaiTro))
            {
                TempData["Loi"] = "Tên vai trò đã tồn tại!";
            }
            else
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Thêm vai trò thành công!";
            }
            return RedirectToAction("QuanLyCauHinhVaiTro");
        }

        [HttpPost]
        public async Task<IActionResult> SuaVaiTro(CauHinhVaiTro model)
        {
            if (_context.CauHinhVaiTro.Any(x => x.TenVaiTro == model.TenVaiTro && x.VaiTroID != model.VaiTroID))
            {
                TempData["Loi"] = "Tên vai trò đã tồn tại!";
            }
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật vai trò thành công!";
            }
            return RedirectToAction("QuanLyCauHinhVaiTro");
        }

        [HttpPost]
        public async Task<IActionResult> XoaVaiTro(int id)
        {
            var vt = await _context.CauHinhVaiTro.FindAsync(id);
            if (vt == null)
            {
                TempData["Loi"] = "Không tìm thấy vai trò cần xóa!";
            }
            else if (_context.NguoiDung.Any(nd => nd.VaiTroID == id))
            {
                TempData["Loi"] = "Không thể xóa vai trò đang được sử dụng!";
            }
            else
            {
                _context.CauHinhVaiTro.Remove(vt);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa vai trò thành công!";
            }
            return RedirectToAction("QuanLyCauHinhVaiTro");
        }
        #endregion
        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Cấu hình Xếp loại
        //=======================================================================================
        #region Cấu hình xếp loại rèn luyện

        [HttpGet]
        public async Task<IActionResult> QuanLyCauHinhXepLoai()
        {
            var list = await _context.CauHinhXepLoai.OrderBy(x => x.DiemToiThieu).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ThemXepLoai(CauHinhXepLoai model)
        {
            if (_context.CauHinhXepLoai.Any(x => x.TenXepLoai == model.TenXepLoai))
            {
                TempData["Loi"] = "Tên xếp loại đã tồn tại!";
            }
            else if (model.DiemToiThieu > model.DiemToiDa)
            {
                TempData["Loi"] = "Điểm tối thiểu phải nhỏ hơn hoặc bằng điểm tối đa!";
            }
            else
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Thêm xếp loại thành công!";
            }
            return RedirectToAction("QuanLyCauHinhXepLoai");
        }

        [HttpPost]
        public async Task<IActionResult> SuaXepLoai(CauHinhXepLoai model)
        {
            if (_context.CauHinhXepLoai.Any(x => x.TenXepLoai == model.TenXepLoai && x.XepLoaiID != model.XepLoaiID))
            {
                TempData["Loi"] = "Tên xếp loại đã tồn tại!";
            }
            else if (model.DiemToiThieu > model.DiemToiDa)
            {
                TempData["Loi"] = "Điểm tối thiểu phải nhỏ hơn hoặc bằng điểm tối đa!";
            }
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật xếp loại thành công!";
            }
            return RedirectToAction("QuanLyCauHinhXepLoai");
        }

        [HttpPost]
        public async Task<IActionResult> XoaXepLoai(int id)
        {
            var xl = await _context.CauHinhXepLoai.FindAsync(id);
            if (xl == null)
            {
                TempData["Loi"] = "Không tìm thấy xếp loại cần xóa!";
            }
            else if (_context.KetQuaRenLuyen.Any(kq => kq.XepLoaiID == id))
            {
                TempData["Loi"] = "Không thể xóa xếp loại đang được sử dụng!";
            }
            else
            {
                _context.CauHinhXepLoai.Remove(xl);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa xếp loại thành công!";
            }
            return RedirectToAction("QuanLyCauHinhXepLoai");
        }

        #endregion
        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Quản lý Trường
        //=======================================================================================
        #region Quản lý Trường

        [HttpGet]
        public async Task<IActionResult> QuanLyTruong()
        {
            var list = await _context.Truong.OrderBy(t => t.TenTruong).ToListAsync();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ThemTruong(Truong model)
        {
            if (_context.Truong.Any(t => t.TenTruong == model.TenTruong))
            {
                TempData["Loi"] = "Tên trường đã tồn tại!";
            }
            else
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Thêm trường thành công!";
            }
            return RedirectToAction("QuanLyTruong");
        }

        [HttpPost]
        public async Task<IActionResult> SuaTruong(Truong model)
        {
            if (_context.Truong.Any(t => t.TenTruong == model.TenTruong && t.TruongID != model.TruongID))
            {
                TempData["Loi"] = "Tên trường đã tồn tại!";
            }
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật trường thành công!";
            }
            return RedirectToAction("QuanLyTruong");
        }

        [HttpPost]
        public async Task<IActionResult> XoaTruong(int id)
        {
            var truong = await _context.Truong.FindAsync(id);
            if (truong == null)
            {
                TempData["Loi"] = "Không tìm thấy trường cần xóa!";
            }
            else if (_context.Khoa.Any(k => k.TruongID == id))
            {
                TempData["Loi"] = "Không thể xóa trường đã có khoa liên kết!";
            }
            else
            {
                _context.Truong.Remove(truong);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa trường thành công!";
            }
            return RedirectToAction("QuanLyTruong");
        }

        #endregion
        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Quản lý Khoa
        //=======================================================================================
        #region Quản lý Khoa

        [HttpGet]
        public async Task<IActionResult> QuanLyKhoa()
        {
            ViewBag.DanhSachTruong = await _context.Truong.ToListAsync();
            var ds = await _context.Khoa.Include(k => k.Truong).OrderBy(k => k.TenKhoa).ToListAsync();
            return View(ds);
        }

        [HttpPost]
        public async Task<IActionResult> ThemKhoa(Khoa model)
        {
            if (_context.Khoa.Any(k => k.TenKhoa == model.TenKhoa && k.TruongID == model.TruongID))
            {
                TempData["Loi"] = "Tên khoa đã tồn tại trong trường này!";
            }
            else
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Thêm khoa thành công!";
            }
            return RedirectToAction("QuanLyKhoa");
        }

        [HttpPost]
        public async Task<IActionResult> SuaKhoa(Khoa model)
        {
            if (_context.Khoa.Any(k => k.TenKhoa == model.TenKhoa && k.TruongID == model.TruongID && k.KhoaID != model.KhoaID))
            {
                TempData["Loi"] = "Tên khoa đã tồn tại trong trường này!";
            }
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật khoa thành công!";
            }
            return RedirectToAction("QuanLyKhoa");
        }

        [HttpPost]
        public async Task<IActionResult> XoaKhoa(int id)
        {
            var khoa = await _context.Khoa.FindAsync(id);
            if (khoa == null)
            {
                TempData["Loi"] = "Không tìm thấy khoa cần xóa!";
            }
            else if (_context.Lop.Any(l => l.KhoaID == id) || _context.SinhVien.Any(s => s.KhoaID == id) || _context.NhanVien.Any(nv => nv.KhoaID == id))
            {
                TempData["Loi"] = "Không thể xóa khoa đã có lớp, sinh viên hoặc nhân viên liên kết!";
            }
            else
            {
                _context.Khoa.Remove(khoa);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa khoa thành công!";
            }
            return RedirectToAction("QuanLyKhoa");
        }

        #endregion
        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Quản lý Niên khóa
        //=======================================================================================
        #region Quản lý Niên khóa

        [HttpGet]
        public async Task<IActionResult> QuanLyNienKhoa()
        {
            var ds = await _context.NienKhoa.OrderByDescending(nk => nk.NamBatDau).ToListAsync();
            return View(ds);
        }

        [HttpPost]
        public async Task<IActionResult> ThemNienKhoa(NienKhoa model)
        {
            bool trung = _context.NienKhoa.Any(nk =>
                nk.TenNienKhoa == model.TenNienKhoa ||
                (nk.NamBatDau == model.NamBatDau && nk.NamKetThuc == model.NamKetThuc));

            if (trung)
            {
                TempData["Loi"] = "Niên khóa đã tồn tại!";
            }
            else
            {
                _context.NienKhoa.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Thêm niên khóa thành công!";
            }

            return RedirectToAction("QuanLyNienKhoa");
        }

        [HttpPost]
        public async Task<IActionResult> SuaNienKhoa(NienKhoa model)
        {
            bool trung = _context.NienKhoa.Any(nk =>
                (nk.TenNienKhoa == model.TenNienKhoa || (nk.NamBatDau == model.NamBatDau && nk.NamKetThuc == model.NamKetThuc))
                && nk.NienKhoaID != model.NienKhoaID);

            if (trung)
            {
                TempData["Loi"] = "Niên khóa đã tồn tại!";
            }
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật niên khóa thành công!";
            }

            return RedirectToAction("QuanLyNienKhoa");
        }

        [HttpPost]
        public async Task<IActionResult> XoaNienKhoa(int id)
        {
            var nk = await _context.NienKhoa.FindAsync(id);
            if (nk == null)
            {
                TempData["Loi"] = "Không tìm thấy niên khóa!";
            }
            else if (_context.Lop.Any(l => l.NienKhoaID == id) || _context.HocKy.Any(hk => hk.NienKhoaID == id))
            {
                TempData["Loi"] = "Không thể xóa niên khóa đã được liên kết với lớp hoặc học kỳ!";
            }
            else
            {
                _context.Remove(nk);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa niên khóa thành công!";
            }

            return RedirectToAction("QuanLyNienKhoa");
        }

        #endregion
        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Quản lý Lớp
        //=======================================================================================
        #region Quản lý Lớp

        [HttpGet]
        public async Task<IActionResult> QuanLyLop()
        {
            ViewBag.Khoas = await _context.Khoa.ToListAsync();
            ViewBag.NienKhoas = await _context.NienKhoa.ToListAsync();

            var ds = await _context.Lop
                .Include(l => l.Khoa)
                .Include(l => l.NienKhoa)
                .ToListAsync();

            return View(ds);
        }

        [HttpPost]
        public async Task<IActionResult> ThemLop(Lop model)
        {
            bool trung = _context.Lop.Any(l =>
                l.TenLop == model.TenLop &&
                l.KhoaID == model.KhoaID &&
                l.NienKhoaID == model.NienKhoaID);

            if (trung)
            {
                TempData["Loi"] = "Lớp đã tồn tại!";
            }
            else
            {
                _context.Lop.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Thêm lớp thành công!";
            }

            return RedirectToAction("QuanLyLop");
        }

        [HttpPost]
        public async Task<IActionResult> SuaLop(Lop model)
        {
            bool trung = _context.Lop.Any(l =>
                l.TenLop == model.TenLop &&
                l.KhoaID == model.KhoaID &&
                l.NienKhoaID == model.NienKhoaID &&
                l.LopID != model.LopID);

            if (trung)
            {
                TempData["Loi"] = "Lớp đã tồn tại!";
            }
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật lớp thành công!";
            }

            return RedirectToAction("QuanLyLop");
        }

        [HttpPost]
        public async Task<IActionResult> XoaLop(int id)
        {
            var lop = await _context.Lop.FindAsync(id);
            if (lop == null)
            {
                TempData["Loi"] = "Không tìm thấy lớp!";
            }
            else if (_context.SinhVien.Any(sv => sv.LopID == id) || _context.ChuNhiem.Any(cn => cn.LopID == id))
            {
                TempData["Loi"] = "Không thể xóa lớp đã được liên kết!";
            }
            else
            {
                _context.Lop.Remove(lop);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa lớp thành công!";
            }

            return RedirectToAction("QuanLyLop");
        }

        #endregion
        //=======================================================================================
        //----------------------------------------------------------------------------------------
        //Quản lý Học kỳ
        //=======================================================================================
        #region Quản lý Học kỳ

        [HttpGet]
        public async Task<IActionResult> QuanLyHocKy()
        {
            ViewBag.NienKhoas = await _context.NienKhoa.ToListAsync();

            var ds = await _context.HocKy.Include(h => h.NienKhoa).ToListAsync();
            return View(ds);
        }

        [HttpPost]
        public async Task<IActionResult> ThemHocKy(HocKy model)
        {
            bool trung = _context.HocKy.Any(h =>
                h.TenHocKy == model.TenHocKy &&
                h.NamHoc == model.NamHoc &&
                h.NienKhoaID == model.NienKhoaID);

            if (trung)
                TempData["Loi"] = "Học kỳ đã tồn tại trong niên khóa này!";
            else
            {
                _context.HocKy.Add(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Thêm học kỳ thành công!";
            }

            return RedirectToAction("QuanLyHocKy");
        }

        [HttpPost]
        public async Task<IActionResult> SuaHocKy(HocKy model)
        {
            bool trung = _context.HocKy.Any(h =>
                h.TenHocKy == model.TenHocKy &&
                h.NamHoc == model.NamHoc &&
                h.NienKhoaID == model.NienKhoaID &&
                h.HocKyID != model.HocKyID);

            if (trung)
                TempData["Loi"] = "Học kỳ bị trùng!";
            else
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật học kỳ thành công!";
            }

            return RedirectToAction("QuanLyHocKy");
        }

        [HttpPost]
        public async Task<IActionResult> XoaHocKy(int id)
        {
            var hocKy = await _context.HocKy.FindAsync(id);
            if (hocKy == null)
                TempData["Loi"] = "Không tìm thấy học kỳ!";
            else if (
                _context.PhieuDanhGia.Any(p => p.HocKyID == id) ||
                _context.ChuNhiem.Any(c => c.HocKyID == id) ||
                _context.KetQuaRenLuyen.Any(k => k.HocKyID == id)
            )
                TempData["Loi"] = "Không thể xóa vì học kỳ đã được sử dụng!";
            else
            {
                _context.HocKy.Remove(hocKy);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa học kỳ thành công!";
            }

            return RedirectToAction("QuanLyHocKy");
        }

        #endregion
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //Quản lý Sinh viên
        //=======================================================================================
        #region Quản lý Sinh viên

        [HttpGet]
        public async Task<IActionResult> QuanLySinhVien()
        {
            ViewBag.Khoas = await _context.Khoa.ToListAsync();
            ViewBag.Lops = await _context.Lop.ToListAsync();
            ViewBag.TrangThaiSinhVien = await _context.CauHinhTrangThaiSinhVien.ToListAsync();

            var ds = await _context.SinhVien
                .Include(s => s.Khoa)
                .Include(s => s.Lop)
                .Include(s => s.TrangThai)
                .ToListAsync();
            return View(ds);
        }

        [HttpPost]
        public async Task<IActionResult> ThemSinhVien(SinhVien model)
        {
            bool tonTai = _context.SinhVien.Any(s => s.MaSV == model.MaSV);
            if (tonTai)
            {
                TempData["Loi"] = "Mã sinh viên đã tồn tại!";
                return RedirectToAction("QuanLySinhVien");
            }

            model.NgayCapNhatTrangThai = DateTime.Now;
            _context.SinhVien.Add(model);
            await _context.SaveChangesAsync();

            // Tạo tài khoản người dùng sau khi lưu sinh viên
            var taiKhoan = new NguoiDung
            {
                Username = model.MaSV,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("1111"),
                VaiTroID = _context.CauHinhVaiTro.FirstOrDefault(v => v.TenVaiTro == "SinhVien")?.VaiTroID ?? 1,
                SinhVienID = model.SinhVienID,
                LastLogin = null
            };

            _context.NguoiDung.Add(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = "Thêm sinh viên và tài khoản thành công!";
            return RedirectToAction("QuanLySinhVien");
        }


        [HttpPost]
        public async Task<IActionResult> SuaSinhVien(SinhVien model)
        {
            bool trungMa = _context.SinhVien.Any(s => s.MaSV == model.MaSV && s.SinhVienID != model.SinhVienID);
            if (trungMa)
                TempData["Loi"] = "Mã sinh viên bị trùng!";
            else
            {
                model.NgayCapNhatTrangThai = DateTime.Now;
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Cập nhật sinh viên thành công!";
            }

            return RedirectToAction("QuanLySinhVien");
        }

        [HttpPost]
        public async Task<IActionResult> XoaSinhVien(int id)
        {
            var sv = await _context.SinhVien.FindAsync(id);
            if (sv == null)
                TempData["Loi"] = "Không tìm thấy sinh viên!";
            else if (
                _context.PhieuDanhGia.Any(p => p.SinhVienID == id) ||
                _context.KetQuaRenLuyen.Any(k => k.SinhVienID == id) ||
                _context.NguoiDung.Any(n => n.SinhVienID == id) ||
                _context.TrangThaiSinhVien.Any(l => l.SinhVienID == id)
            )
                TempData["Loi"] = "Không thể xóa sinh viên vì có dữ liệu liên quan!";
            else
            {
                _context.SinhVien.Remove(sv);
                await _context.SaveChangesAsync();
                TempData["ThanhCong"] = "Xóa sinh viên thành công!";
            }

            return RedirectToAction("QuanLySinhVien");
        }

        #endregion
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        //Quản lý Nhân viên
        //=======================================================================================
        public IActionResult QuanLyNhanVien()
        {
            var list = _context.NhanVien
                .Include(n => n.Khoa)
                .Include(n => n.CauHinhVaiTro)
                .ToList();                     

            ViewBag.KhoaList = _context.Khoa.ToList();
            ViewBag.VaiTroList = _context.CauHinhVaiTro.ToList();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ThemNhanVien(NhanVien model)
        {
            bool tonTai = _context.NhanVien.Any(n => n.MaNV == model.MaNV);
            if (tonTai)
            {
                TempData["Loi"] = "Mã nhân viên đã tồn tại!";
                return RedirectToAction("QuanLyNhanVien");
            }

            _context.NhanVien.Add(model);
            await _context.SaveChangesAsync();

            // Tạo tài khoản người dùng
            var taiKhoan = new NguoiDung
            {
                Username = model.MaNV,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("0000"),
                VaiTroID = model.VaiTroID, 
                NhanVienID = model.NhanVienID,
                LastLogin = null
            };

            _context.NguoiDung.Add(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = "Thêm nhân viên và tài khoản thành công!";
            return RedirectToAction("QuanLyNhanVien");
        }


        [HttpPost]
        public IActionResult SuaNhanVien(int NhanVienID, string MaNV, string HoTen, int? KhoaID, int VaiTroID)
        {
            var nv = _context.NhanVien.Find(NhanVienID);
            if (nv == null)
            {
                TempData["Loi"] = "Không tìm thấy nhân viên.";
                return RedirectToAction("QuanLyNhanVien");
            }

            if (_context.NhanVien.Any(n => n.MaNV == MaNV && n.NhanVienID != NhanVienID))
            {
                TempData["Loi"] = "Mã nhân viên đã tồn tại.";
                return RedirectToAction("QuanLyNhanVien");
            }

            nv.MaNV = MaNV;
            nv.HoTen = HoTen;
            nv.KhoaID = KhoaID;
            _context.SaveChanges();

            // Cập nhật VaiTrò trong bảng người dùng
            var nguoiDung = _context.NguoiDung.FirstOrDefault(nd => nd.NhanVienID == NhanVienID);
            if (nguoiDung != null)
            {
                nguoiDung.VaiTroID = VaiTroID;
                _context.SaveChanges();
            }

            TempData["ThanhCong"] = "Đã cập nhật nhân viên.";
            return RedirectToAction("QuanLyNhanVien");
        }

        [HttpPost]
        public IActionResult XoaNhanVien(int id)
        {
            var nv = _context.NhanVien.Find(id);
            if (nv == null)
            {
                TempData["Loi"] = "Không tìm thấy nhân viên.";
            }
            else if (_context.ChuNhiem.Any(cn => cn.NhanVienID == id) || _context.NguoiDung.Any(nd => nd.NhanVienID == id))
            {
                TempData["Loi"] = "Không thể xóa vì nhân viên này đang được sử dụng.";
            }
            else
            {
                _context.NhanVien.Remove(nv);
                _context.SaveChanges();
                TempData["ThanhCong"] = "Đã xóa nhân viên.";
            }
            return RedirectToAction("QuanLyNhanVien");
        }
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //Quản lý Người dùng
        //=======================================================================================
        // GET: Quản lý người dùng
        public IActionResult QuanLyNguoiDung()
        {
            var list = _context.NguoiDung
                .Include(x => x.VaiTro)
                .Include(x => x.SinhVien)
                .Include(x => x.NhanVien)
                .ToList();

            ViewBag.VaiTros = _context.CauHinhVaiTro.ToList();
            ViewBag.SinhViens = _context.SinhVien.ToList();
            ViewBag.NhanViens = _context.NhanVien.ToList();

            return View(list);
        }

        // POST: Thêm
        [HttpPost]
        public IActionResult ThemNguoiDung(string Username, string Password, int VaiTroID, int? SinhVienID, int? NhanVienID)
        {
            if (_context.NguoiDung.Any(x => x.Username == Username))
            {
                TempData["Loi"] = "Tên đăng nhập đã tồn tại.";
                return RedirectToAction("QuanLyNguoiDung");
            }

            var nd = new NguoiDung
            {
                Username = Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password),
                VaiTroID = VaiTroID,
                SinhVienID = SinhVienID,
                NhanVienID = NhanVienID,
                LastLogin = null
            };

            _context.NguoiDung.Add(nd);
            _context.SaveChanges();

            TempData["ThanhCong"] = "Đã thêm người dùng.";
            return RedirectToAction("QuanLyNguoiDung");
        }
        // POST: Sửa
        [HttpPost]
        public IActionResult CapNhatMatKhau(int id, string Password)
        {
            var nd = _context.NguoiDung.Find(id);
            if (nd == null)
            {
                TempData["Loi"] = "Không tìm thấy người dùng!";
                return RedirectToAction("QuanLyNguoiDung");
            }

            nd.PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password);
            _context.Update(nd);
            _context.SaveChanges();

            TempData["ThanhCong"] = "Cập nhật mật khẩu thành công!";
            return RedirectToAction("QuanLyNguoiDung");
        }

        // POST: Xóa
        [HttpPost]
        public IActionResult XoaNguoiDung(int id)
        {
            var nd = _context.NguoiDung.Find(id);
            if (nd == null)
            {
                TempData["Loi"] = "Không tìm thấy người dùng.";
                return RedirectToAction("QuanLyNguoiDung");
            }

            // TODO: Kiểm tra ràng buộc nếu cần

            _context.NguoiDung.Remove(nd);
            _context.SaveChanges();
            TempData["ThanhCong"] = "Đã xóa người dùng.";
            return RedirectToAction("QuanLyNguoiDung");
        }
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //Quản lý Nhóm tiêu chí
        //=======================================================================================
        // === QUẢN LÝ NHÓM TIÊU CHÍ ===
        public IActionResult QuanLyNhomTieuChi()
        {
            var ds = _context.NhomTieuChi.OrderBy(x => x.NhomTieuChiID).ToList();
            return View(ds);
        }

        [HttpPost]
        public IActionResult ThemNhomTieuChi(NhomTieuChi model)
        {
            if (ModelState.IsValid)
            {
                if (_context.NhomTieuChi.Any(x => x.TenNhom == model.TenNhom))
                {
                    TempData["Error"] = "Tên nhóm tiêu chí đã tồn tại.";
                    return RedirectToAction("QuanLyNhomTieuChi");
                }

                _context.NhomTieuChi.Add(model);
                _context.SaveChanges();
                TempData["Success"] = "Đã thêm nhóm tiêu chí.";
            }
            return RedirectToAction("QuanLyNhomTieuChi");
        }

        [HttpPost]
        public IActionResult SuaNhomTieuChi(NhomTieuChi model)
        {
            if (ModelState.IsValid)
            {
                var old = _context.NhomTieuChi.Find(model.NhomTieuChiID);
                if (old == null)
                {
                    TempData["Error"] = "Không tìm thấy nhóm.";
                }
                else
                {
                    old.TenNhom = model.TenNhom;
                    old.DiemToiDa = model.DiemToiDa;
                    _context.SaveChanges();
                    TempData["Success"] = "Đã cập nhật nhóm tiêu chí.";
                }
            }
            return RedirectToAction("QuanLyNhomTieuChi");
        }

        public IActionResult XoaNhomTieuChi(int id)
        {
            var nhom = _context.NhomTieuChi.Include(x => x.TieuChi).FirstOrDefault(x => x.NhomTieuChiID == id);
            if (nhom == null)
            {
                TempData["Error"] = "Không tìm thấy nhóm.";
            }
            else if (nhom.TieuChi != null && nhom.TieuChi.Any())
            {
                TempData["Error"] = "Không thể xóa nhóm đã có tiêu chí.";
            }
            else
            {
                _context.NhomTieuChi.Remove(nhom);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa nhóm tiêu chí.";
            }
            return RedirectToAction("QuanLyNhomTieuChi");
        }
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //Quản lý Tiêu chí
        //=======================================================================================
        // Trong AdminController.cs

        // === QUẢN LÝ TIÊU CHÍ ===
        public IActionResult QuanLyTieuChi()
        {
            var ds = _context.TieuChi.Include(t => t.NhomTieuChi).ToList();
            ViewBag.NhomTieuChiList = _context.NhomTieuChi.ToList();
            return View(ds);
        }

        [HttpPost]
        public IActionResult ThemTieuChi(TieuChi model)
        {
            if (ModelState.IsValid)
            {
                if (_context.TieuChi.Any(x => x.TenTieuChi == model.TenTieuChi && x.NhomTieuChiID == model.NhomTieuChiID))
                {
                    TempData["Error"] = "Tiêu chí đã tồn tại trong nhóm.";
                }
                else
                {
                    _context.TieuChi.Add(model);
                    _context.SaveChanges();
                    TempData["Success"] = "Đã thêm tiêu chí.";
                }
            }
            return RedirectToAction("QuanLyTieuChi");
        }

        [HttpPost]
        public IActionResult SuaTieuChi(TieuChi model)
        {
            var tc = _context.TieuChi.Find(model.TieuChiID);
            if (tc == null)
            {
                TempData["Error"] = "Không tìm thấy tiêu chí.";
            }
            else
            {
                tc.NhomTieuChiID = model.NhomTieuChiID;
                tc.TenTieuChi = model.TenTieuChi;
                tc.DiemToiDa = model.DiemToiDa;
                _context.SaveChanges();
                TempData["Success"] = "Đã cập nhật tiêu chí.";
            }
            return RedirectToAction("QuanLyTieuChi");
        }

        public IActionResult XoaTieuChi(int id)
        {
            var tc = _context.TieuChi.FirstOrDefault(x => x.TieuChiID == id);
            if (tc == null)
            {
                TempData["Error"] = "Không tìm thấy tiêu chí.";
            }
            else
            {
                var isUsed = _context.ChiTietPhieuDanhGia.Any(ct => ct.TieuChiID == id);
                if (isUsed)
                {
                    TempData["Error"] = "Không thể xóa tiêu chí đã được sử dụng.";
                }
                else
                {
                    _context.TieuChi.Remove(tc);
                    _context.SaveChanges();
                    TempData["Success"] = "Đã xóa tiêu chí.";
                }
            }
            return RedirectToAction("QuanLyTieuChi");
        }

        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //Quản lý Phiếu đánh giá
        //=======================================================================================
        [HttpGet]
        public async Task<IActionResult> QuanLyPhieuDanhGia(int? NienKhoaID, int? KhoaID, int? HocKyID, int? LopID, string MaSV)
        {
            // Lấy danh sách cho dropdown
            var nienKhoas = await _context.NienKhoa.OrderBy(nk => nk.TenNienKhoa).ToListAsync();
            var khoas = await _context.Khoa.OrderBy(k => k.TenKhoa).ToListAsync();
            var hocKys = await _context.HocKy.Include(hk => hk.NienKhoa).OrderBy(hk => hk.NamHoc).ToListAsync();
            var lops = await _context.Lop.Include(l => l.Khoa).OrderBy(l => l.TenLop).ToListAsync();
            var trangThai = await _context.CauHinhTrangThaiSinhVien.ToListAsync();

            // Kiểm tra dữ liệu dropdown
            if (nienKhoas.Count == 0)
            {
                TempData["Loi"] = "Không có dữ liệu năm học trong cơ sở dữ liệu.";
            }
            if (khoas.Count == 0)
            {
                TempData["Loi"] = TempData["Loi"]?.ToString() + " Không có dữ liệu khoa trong cơ sở dữ liệu.";
            }

            ViewBag.NienKhoas = nienKhoas;
            ViewBag.Khoas = khoas;
            ViewBag.HocKys = hocKys;
            ViewBag.Lops = lops;
            ViewBag.TrangThai = trangThai;
            ViewBag.MaSV = MaSV; // Lưu MaSV để hiển thị lại trong ô input
            ViewBag.NienKhoaID = NienKhoaID;
            ViewBag.KhoaID = KhoaID;
            ViewBag.HocKyID = HocKyID;
            ViewBag.LopID = LopID;

            // Xây dựng query cho phiếu đánh giá
            var query = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(sv => sv!.Lop).ThenInclude(l => l!.Khoa)
                .Include(p => p.HocKy).ThenInclude(hk => hk!.NienKhoa)
                .Include(p => p.TrangThaiDanhGia)
                .AsQueryable();

            // Áp dụng bộ lọc nếu có
            bool hasFilter = false;

            if (!string.IsNullOrEmpty(MaSV))
            {
                query = query.Where(p => p.SinhVien!.MaSV.Contains(MaSV.Trim()));
                hasFilter = true;
            }

            if (NienKhoaID.HasValue)
            {
                query = query.Where(p => p.HocKy!.NienKhoaID == NienKhoaID.Value);
                hasFilter = true;
            }

            if (KhoaID.HasValue)
            {
                query = query.Where(p => p.SinhVien!.KhoaID == KhoaID.Value);
                hasFilter = true;
            }

            if (HocKyID.HasValue)
            {
                query = query.Where(p => p.HocKyID == HocKyID.Value);
                hasFilter = true;
            }

            if (LopID.HasValue)
            {
                query = query.Where(p => p.SinhVien!.LopID == LopID.Value);
                hasFilter = true;
            }

            var result = await query.OrderBy(p => p.NgayLapPhieu).ToListAsync();

            // Hiển thị thông báo nếu không tìm thấy dữ liệu
            if (hasFilter && result.Count == 0)
            {
                ViewBag.KhongTimThay = "Không tìm thấy phiếu đánh giá phù hợp với tiêu chí đã chọn.";
            }

            return View(result);
        }

        [HttpPost]
        public IActionResult XoaPhieuDanhGia(int id)
        {
            var phieu = _context.PhieuDanhGia
                .Include(p => p.SinhVien)
                .FirstOrDefault(p => p.PhieuDanhGiaID == id);

            if (phieu == null)
            {
                TempData["Loi"] = "Không tìm thấy phiếu.";
                return RedirectToAction("QuanLyPhieuDanhGia");
            }

            // Kiểm tra ràng buộc
            bool coLienKet = _context.ChiTietPhieuDanhGia.Any(c => c.PhieuDanhGiaID == id)
                          || _context.KetQuaRenLuyen.Any(k => k.PhieuDanhGiaID == id);

            if (coLienKet)
            {
                TempData["Loi"] = "Không thể xóa: Phiếu có liên kết chi tiết hoặc kết quả.";
                return RedirectToAction("QuanLyPhieuDanhGia");
            }

            _context.PhieuDanhGia.Remove(phieu);
            _context.SaveChanges();
            TempData["ThanhCong"] = "Đã xóa phiếu.";
            return RedirectToAction("QuanLyPhieuDanhGia");
        }

        [HttpGet]
        public IActionResult ChiTietPhieu(int id)
        {
            TempData["Loi"] = "Vui lòng sử dụng nút 'Chi tiết' trên bảng để xem thông tin.";
            return RedirectToAction("QuanLyPhieuDanhGia");
        }
        //=======================================================================================
        //ĐANG LỦNG CỦNG PHẦN NÀY, ĐỌC LẠI
        //------------------------------------------------------------------------------------------
        //Quản lý Chi tiết phiếu đánh giá
        //=======================================================================================
        // GET: Chi tiết phiếu đánh giá
        public IActionResult QuanLyChiTietPhieuDanhGia(string? namHoc, string? hocKy, int? khoaId, int? lopId, string? maSV)
        {
            var query = _context.ChiTietPhieuDanhGia
                .Include(c => c.PhieuDanhGia).ThenInclude(p => p!.SinhVien)
                .Include(c => c.TieuChi).ThenInclude(tc => tc!.NhomTieuChi)
                .Include(c => c.PhieuDanhGia).ThenInclude(p => p!.HocKy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(namHoc))
            {
                query = query.Where(c => c.PhieuDanhGia!.HocKy!.NamHoc == namHoc);
            }

            if (!string.IsNullOrEmpty(hocKy))
            {
                query = query.Where(c => c.PhieuDanhGia!.HocKy!.TenHocKy == hocKy);
            }

            if (khoaId.HasValue)
            {
                query = query.Where(c => c.PhieuDanhGia!.SinhVien!.KhoaID == khoaId.Value);
            }

            if (lopId.HasValue)
            {
                query = query.Where(c => c.PhieuDanhGia!.SinhVien!.LopID == lopId.Value);
            }

            if (!string.IsNullOrEmpty(maSV))
            {
                query = query.Where(c => c.PhieuDanhGia!.SinhVien!.MaSV.Contains(maSV));
            }

            var data = query.ToList();

            // Đổ dữ liệu dropdown
            ViewBag.NamHocs = _context.HocKy.Select(h => h.NamHoc).Distinct().OrderByDescending(n => n).ToList();
            ViewBag.HocKys = _context.HocKy.Select(h => h.TenHocKy).Distinct().ToList();
            ViewBag.Khoas = _context.Khoa.OrderBy(k => k.TenKhoa).ToList();
            ViewBag.Lops = _context.Lop.OrderBy(l => l.TenLop).ToList();

            ViewBag.BangThongTin = data;
            return View("QuanLyChiTietPhieuDanhGia", data);
        }


        // GET: Xóa
        [HttpPost]
        public async Task<IActionResult> XoaNhieuChiTietPhieu(List<int> selectedIds)
        {
            if (selectedIds == null || selectedIds.Count == 0)
            {
                TempData["Error"] = "Bạn chưa chọn chi tiết nào để xóa.";
                return RedirectToAction("QuanLyChiTietPhieuDanhGia");
            }

            int soLuongXoa = 0;
            try
            {
                foreach (var id in selectedIds)
                {
                    var item = await _context.ChiTietPhieuDanhGia.FindAsync(id);
                    if (item != null)
                    {
                        _context.ChiTietPhieuDanhGia.Remove(item);
                        soLuongXoa++;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa {soLuongXoa} chi tiết phiếu.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa: " + ex.Message;
            }

            return RedirectToAction("QuanLyChiTietPhieuDanhGia");
        }
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //Quản lý Kết quả rèn luyện
        //=======================================================================================
        // GET: Danh sách kết quả rèn luyện
        public IActionResult QuanLyKetQuaRenLuyen()
        {
            var data = _context.KetQuaRenLuyen
                .Include(k => k.SinhVien)
                .Include(k => k.HocKy)
                .Include(k => k.XepLoai)
                .Include(k => k.PhieuDanhGia)
                .ToList();

            return View("QuanLyKetQuaRenLuyen", data);
        }

        // GET: Xóa kết quả
        public async Task<IActionResult> XoaKetQuaRenLuyen(int id)
        {
            var item = await _context.KetQuaRenLuyen.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            try
            {
                _context.KetQuaRenLuyen.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa thành công.";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa do ràng buộc dữ liệu.";
            }

            return RedirectToAction("QuanLyKetQuaRenLuyen");
        }
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //Quản lý lịch sử trạng thái SV
        //=======================================================================================
        // GET: Danh sách lịch sử trạng thái SV
        public IActionResult QuanLyTrangThaiSinhVien()
        {
            var data = _context.TrangThaiSinhVien
                .Include(ls => ls.SinhVien)
                .Include(ls => ls.TrangThai)
                //.Include(ls => ls.NgayCapNhat)
                .ToList();

            return View("QuanLyTrangThaiSinhVien", data);
        }

        [HttpPost]
        public IActionResult SuaLichSuTrangThai(int LichSuID, int TrangThaiID)
        {
            var ls = _context.TrangThaiSinhVien.FirstOrDefault(x => x.LichSuID == LichSuID);
            if (ls == null)
            {
                TempData["Error"] = "Không tìm thấy lịch sử cần sửa.";
                return RedirectToAction("QuanLyTrangThaiSinhVien");
            }

            ls.TrangThaiID = TrangThaiID;
            ls.NgayCapNhat = DateTime.Now;
            _context.SaveChanges();

            TempData["Success"] = "Cập nhật trạng thái thành công.";
            return RedirectToAction("QuanLyTrangThaiSinhVien");
        }


        // GET: Xóa
        public async Task<IActionResult> XoaLichSuTrangThai(int id)
        {
            var item = await _context.TrangThaiSinhVien.FindAsync(id);
            if (item == null) return NotFound();

            try
            {
                _context.TrangThaiSinhVien.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa thành công.";
            }
            catch
            {
                TempData["Error"] = "Không thể xóa do ràng buộc dữ liệu.";
            }

            return RedirectToAction("QuanLyTrangThaiSinhVien");
        }

        //=======================================================================================

        // Replace the obsolete 'HtmlToPdf' usage with 'ChromePdfRenderer' as per the diagnostic message.
        public IActionResult XuatDanhSachDiem(int? LopID, int? HocKyID, string type)
        {
            var query = _context.KetQuaRenLuyen
                .Include(k => k.SinhVien)
                .ThenInclude(sv => sv!.Lop)
                .Include(k => k.HocKy)
                .Include(k => k.XepLoai)
                .Where(k => (LopID == null || k.SinhVien!.LopID == LopID)
                         && (HocKyID == null || k.HocKyID == HocKyID))
                .Select(k => new
                {
                    k.SinhVien!.HoTen,
                    k.SinhVien.MaSV,
                    Lop = k.SinhVien.Lop!.TenLop,
                    HocKy = k.HocKy!.TenHocKy + " " + k.HocKy.NamHoc,
                    Diem = k.TongDiemHoiDongDuyet,
                    XepLoai = k.XepLoai!.TenXepLoai
                })
                .ToList();

            if (type == "excel")
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook();
                var worksheet = workbook.Worksheets.Add("DiemRenLuyen");
                worksheet.Cell(1, 1).Value = "Họ tên";
                worksheet.Cell(1, 2).Value = "Mã SV";
                worksheet.Cell(1, 3).Value = "Lớp";
                worksheet.Cell(1, 4).Value = "Học kỳ";
                worksheet.Cell(1, 5).Value = "Điểm";
                worksheet.Cell(1, 6).Value = "Xếp loại";

                for (int i = 0; i < query.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = query[i].HoTen;
                    worksheet.Cell(i + 2, 2).Value = query[i].MaSV;
                    worksheet.Cell(i + 2, 3).Value = query[i].Lop;
                    worksheet.Cell(i + 2, 4).Value = query[i].HocKy;
                    worksheet.Cell(i + 2, 5).Value = query[i].Diem;
                    worksheet.Cell(i + 2, 6).Value = query[i].XepLoai;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DiemRenLuyen.xlsx");
            }
            else if (type == "pdf")
            {
                var html = "<h3>DANH SÁCH ĐIỂM RÈN LUYỆN</h3><table border='1' cellpadding='5'><tr><th>Họ tên</th><th>Mã SV</th><th>Lớp</th><th>Học kỳ</th><th>Điểm</th><th>Xếp loại</th></tr>";
                foreach (var item in query)
                {
                    html += $"<tr><td>{item.HoTen}</td><td>{item.MaSV}</td><td>{item.Lop}</td><td>{item.HocKy}</td><td>{item.Diem}</td><td>{item.XepLoai}</td></tr>";
                }
                html += "</table>";

                // Use ChromePdfRenderer instead of the obsolete HtmlToPdf
                ////var renderer = new IronPdf.ChromePdfRenderer();
                //var pdf = renderer.RenderHtmlAsPdf(html);
                //return File(pdf.BinaryData, "application/pdf", "DiemRenLuyen.pdf");
            }

            return RedirectToAction("QuanLyThongBao");
        }

        //=======================================================================================
        //------------------------------------------------------------------------------------------
        // Thống kê sinh viên chưa đánh giá
        [HttpGet]
        public IActionResult ThongKeChuaDanhGia(int? HocKyID, int? KhoaID, int? LopID, string? Loai)
        {
            // Lấy danh sách học kỳ và khoa
            var hocKys = _context.HocKy.Include(h => h.NienKhoa).OrderBy(h => h.NamHoc).ToList();
            var khoas = _context.Khoa.OrderBy(k => k.TenKhoa).ToList();

            // Lấy toàn bộ danh sách lớp
            var dsLop = _context.Lop
                .OrderBy(l => l.TenLop)
                .ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.HocKys = hocKys;
            ViewBag.Khoas = khoas;
            ViewBag.Lops = dsLop;
            ViewBag.HocKyID = HocKyID;
            ViewBag.KhoaID = KhoaID;
            ViewBag.LopID = LopID;
            ViewBag.Loai = Loai;

            // Kiểm tra nếu chọn Loại mà không chọn Học kỳ
            if (!string.IsNullOrEmpty(Loai) && !HocKyID.HasValue)
            {
                TempData["Error"] = "Chưa chọn học kỳ!";
                return View("ThongKeChuaDanhGia", new List<SinhVien>());
            }

            // Lấy danh sách sinh viên
            var sinhViens = _context.SinhVien
                .Include(sv => sv.Lop).ThenInclude(l => l!.Khoa)
                .Include(sv => sv.KetQuaRenLuyen)
                .AsQueryable();

            // Lọc theo Khoa (nếu có)
            if (KhoaID.HasValue)
                sinhViens = sinhViens.Where(sv => sv.KhoaID == KhoaID);

            // Lọc theo Lớp (nếu có)
            if (LopID.HasValue)
                sinhViens = sinhViens.Where(sv => sv.LopID == LopID);

            // Lọc theo Loại và Học kỳ
            List<SinhVien> ketQua = new();
            if (HocKyID.HasValue)
            {
                if (!string.IsNullOrEmpty(Loai))
                {
                    if (Loai == "ChuaDat")
                    {
                        ketQua = sinhViens
                            .Where(sv => sv.KetQuaRenLuyen!.Any(kq =>
                                kq.HocKyID == HocKyID && kq.TongDiemHoiDongDuyet <= 49))
                            .ToList();
                    }
                    else if (Loai == "ChuaCoDiem")
                    {
                        ketQua = sinhViens
                            .Where(sv => !sv.KetQuaRenLuyen!.Any(kq => kq.HocKyID == HocKyID))
                            .ToList();
                    }
                }
                else
                {
                    // Nếu chỉ chọn Học kỳ (hoặc kết hợp với Khoa/Lớp), trả về tất cả sinh viên trong học kỳ đó
                    ketQua = sinhViens
                        .Where(sv => sv.KetQuaRenLuyen!.Any(kq => kq.HocKyID == HocKyID))
                        .ToList();
                }
            }
            else if (KhoaID.HasValue || LopID.HasValue)
            {
                // Nếu chỉ chọn Khoa hoặc Lớp, trả về tất cả sinh viên theo Khoa/Lớp
                ketQua = sinhViens.ToList();
            }
            else
            {
                // Nếu không chọn gì, trả về danh sách rỗng
                TempData["Error"] = "Vui lòng chọn ít nhất một tiêu chí lọc!";
                ketQua = new List<SinhVien>();
            }

            return View("ThongKeChuaDanhGia", ketQua);
        }
        //=======================================================================================
        //------------------------------------------------------------------------------------------
        //=======================================================================================
        // GET: Quản lý Chủ nhiệm
        public IActionResult QuanLyChuNhiem()
        {
            var ds = _context.ChuNhiem
                .Include(c => c.NhanVien).ThenInclude(n => n!.Khoa)
                .Include(c => c.Lop).ThenInclude(l => l!.Khoa)
                .Include(c => c.HocKy).ThenInclude(h => h!.NienKhoa)
                .ToList();

            ViewBag.NhanViens = _context.NhanVien.Include(n => n.Khoa).ToList();
            ViewBag.Lops = _context.Lop.Include(l => l.Khoa).ToList();
            ViewBag.HocKys = _context.HocKy.Include(h => h.NienKhoa).ToList();

            return View(ds);
        }

        // POST: Thêm Chủ nhiệm
        [HttpPost]
        public IActionResult ThemChuNhiem(int NhanVienID, int LopID, int HocKyID, string? GhiChu)
        {
            var nhanVien = _context.NhanVien.FirstOrDefault(n => n.NhanVienID == NhanVienID);
            var lop = _context.Lop.FirstOrDefault(l => l.LopID == LopID);

            if (nhanVien == null || lop == null)
            {
                TempData["Error"] = "Không tìm thấy dữ liệu.";
                return RedirectToAction("QuanLyChuNhiem");
            }

            if (nhanVien.KhoaID != lop.KhoaID)
            {
                TempData["Error"] = "Nhân viên và lớp không thuộc cùng khoa.";
                return RedirectToAction("QuanLyChuNhiem");
            }

            var tonTai = _context.ChuNhiem.Any(c => c.LopID == LopID && c.HocKyID == HocKyID);
            if (tonTai)
            {
                TempData["Error"] = "Lớp đã có chủ nhiệm trong học kỳ này.";
                return RedirectToAction("QuanLyChuNhiem");
            }

            var cn = new ChuNhiem
            {
                NhanVienID = NhanVienID,
                LopID = LopID,
                HocKyID = HocKyID,
                GhiChu = string.IsNullOrWhiteSpace(GhiChu) ? "Không có" : GhiChu
            };

            _context.ChuNhiem.Add(cn);
            _context.SaveChanges();

            TempData["Success"] = "Đã thêm chủ nhiệm thành công.";
            return RedirectToAction("QuanLyChuNhiem");
        }

        // POST: Sửa Chủ nhiệm
        [HttpPost]
        public IActionResult SuaChuNhiem(int ChuNhiemID, int NhanVienID, int LopID, int HocKyID, string? GhiChu)
        {
            var cn = _context.ChuNhiem.FirstOrDefault(c => c.ChuNhiemID == ChuNhiemID);
            var nhanVien = _context.NhanVien.FirstOrDefault(n => n.NhanVienID == NhanVienID);
            var lop = _context.Lop.FirstOrDefault(l => l.LopID == LopID);

            if (cn == null || nhanVien == null || lop == null)
            {
                TempData["Error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("QuanLyChuNhiem");
            }

            if (nhanVien.KhoaID != lop.KhoaID)
            {
                TempData["Error"] = "Nhân viên và lớp không thuộc cùng khoa.";
                return RedirectToAction("QuanLyChuNhiem");
            }

            // Kiểm tra trùng (trừ chính nó)
            bool daTonTai = _context.ChuNhiem.Any(c =>
                c.LopID == LopID &&
                c.HocKyID == HocKyID &&
                c.ChuNhiemID != ChuNhiemID);

            if (daTonTai)
            {
                TempData["Error"] = "Lớp này đã có chủ nhiệm trong học kỳ được chọn.";
                return RedirectToAction("QuanLyChuNhiem");
            }

            // Cập nhật
            cn.NhanVienID = NhanVienID;
            cn.LopID = LopID;
            cn.HocKyID = HocKyID;
            cn.GhiChu = string.IsNullOrWhiteSpace(GhiChu) ? "Không có" : GhiChu;

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật chủ nhiệm thành công.";
            return RedirectToAction("QuanLyChuNhiem");
        }

        // POST: Xóa chủ nhiệm
        [HttpPost]
        public IActionResult XoaChuNhiem(int id)
        {
            var cn = _context.ChuNhiem.Find(id);
            if (cn != null)
            {
                _context.ChuNhiem.Remove(cn);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa thành công.";
            }
            return RedirectToAction("QuanLyChuNhiem");
        }


    }
}
