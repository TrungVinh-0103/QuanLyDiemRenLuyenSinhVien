using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.Models.CauHinh;
using QLDiemRenLuyen.ViewModels;
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
        // GET: Quan ly nguoi dung
        //==============================================================================
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
        //POST: Them nguoi dung
        //==============================================================================
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
        // Xoa nguoi dung
        //==============================================================================
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
        // Reset mat khau cho nguoi dung
        //==============================================================================
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
        // Lấy danh sách sinh viên
        //==============================================================================
        public IActionResult QuanLySinhVien()
        {
            var danhSachSV = _context.SinhVien
                .Include(sv => sv.Lop)
                .ThenInclude(l => l!.Khoa)
                .ToList();

            ViewBag.Lop = _context.Lop
                .Include(l => l.Khoa)
                .Include(l => l.NienKhoa)
                .ToList();

            return View(danhSachSV);
        }

        // POST: Thêm sinh viên
        //==============================================================================
        [HttpPost]
public IActionResult ThemSinhVien(string MaSV, string HoTen, DateTime NgaySinh, int LopID, string TrangThai ,string Email, string password)
{
    // Bắt đầu một transaction để đảm bảo tính toàn vẹn dữ liệu
    using (var transaction = _context.Database.BeginTransaction())
    {
        try
        {
            // 1. Thêm SinhVien
            var sv = new SinhVien
            {
                MaSV = MaSV,
                HoTen = HoTen,
                NgaySinh = NgaySinh,
                LopID = LopID,
                TrangThai = TrangThai,
                Email = Email,
                NgayCapNhatTrangThai = DateTime.Now
            };
            _context.SinhVien.Add(sv);
            _context.SaveChanges(); // Lưu SinhVien trước để có sv.SinhVienID

            // 2. Thêm LichSuTrangThai
            _context.LichSuTrangThai.Add(new LichSuTrangThai
            {
                SinhVienID = sv.SinhVienID,
                TrangThai = TrangThai,
                NgayCapNhat = DateTime.Now,
                GhiChu = "Tạo mới sinh viên"
            });
            // Không cần SaveChanges ở đây nếu muốn gộp chung với NguoiDung

            // 3. Tạo tài khoản người dùng
            // KIỂM TRA TÊN ĐĂNG NHẬP (MaSV) CÓ TỒN TẠI HAY KHÔNG
            var existingUser = _context.NguoiDung.FirstOrDefault(u => u.Username == MaSV);
            if (existingUser != null)
            {
                transaction.Rollback(); // Hoàn tác các thay đổi nếu Username đã tồn tại
                TempData["ErrorMessage"] = $"Lỗi: Mã sinh viên '{MaSV}' đã được sử dụng làm tên đăng nhập cho một tài khoản khác. Vui lòng kiểm tra lại.";
                return RedirectToAction("QuanLySinhVien");
            }

            var user = new NguoiDung
            {
                Username = MaSV,
                PasswordHash = string.IsNullOrEmpty(password) ? "1111" : password,
                VaiTro = "SinhVien",
                SinhVienID = sv.SinhVienID
            };
            _context.NguoiDung.Add(user);

            // Lưu tất cả các thay đổi còn lại (LichSuTrangThai và NguoiDung)
            _context.SaveChanges();

            transaction.Commit(); // Hoàn tất transaction nếu mọi thứ thành công
            TempData["ThongBao"] = "Đã thêm sinh viên, tạo tài khoản và ghi lịch sử trạng thái thành công.";
        }
        catch (DbUpdateException ex)
        {
            transaction.Rollback(); // Hoàn tác transaction khi có lỗi
            // Log the exception details for debugging purposes
            // Console.WriteLine(ex.InnerException?.Message); // Hoặc dùng logger
            TempData["ErrorMessage"] = "Đã xảy ra lỗi khi lưu dữ liệu. Vui lòng thử lại. Chi tiết lỗi: " + ex.InnerException?.Message;
        }
        catch (Exception ex)
        {
            transaction.Rollback(); // Hoàn tác transaction khi có lỗi
            // Log the general exception
            TempData["ErrorMessage"] = "Đã xảy ra lỗi không xác định. Vui lòng liên hệ quản trị viên. Chi tiết lỗi: " + ex.Message;
        }
    }

    return RedirectToAction("QuanLySinhVien");
}
        // POST: Sửa sinh viên
        //==============================================================================
        [HttpPost]
        public IActionResult SuaSinhVien(int SinhVienID, string HoTen, DateTime NgaySinh, int LopID, string TrangThai, string Email)
        {
            var sv = _context.SinhVien.Find(SinhVienID);
            if (sv != null)
            {
                //// Nếu trạng thái thay đổi thì ghi lịch sử
                //if (sv.TrangThai != TrangThai)
                //{
                //    var lichSu = new LichSuTrangThai
                //    {
                //        SinhVienID = sv.SinhVienID,
                //        TrangThai = TrangThai,
                //        NgayCapNhat = DateTime.Now,
                //        GhiChu = "Cập nhật trạng thái từ admin"
                //    };
                //    _context.LichSuTrangThai.Add(lichSu);
                //}
                bool thayDoi = sv.TrangThai != TrangThai;

                sv.HoTen = HoTen;
                sv.NgaySinh = NgaySinh;
                sv.LopID = LopID;
                sv.TrangThai = TrangThai;
                sv.Email = Email;
                sv.NgayCapNhatTrangThai = DateTime.Now;

                if (thayDoi)
                {
                    _context.LichSuTrangThai.Add(new LichSuTrangThai
                    {
                        SinhVienID = SinhVienID,
                        TrangThai = TrangThai,
                        NgayCapNhat = DateTime.Now,
                        GhiChu = "Cập nhật từ admin"
                    });
                }

                _context.SaveChanges();
                TempData["ThongBao"] = "Đã cập nhật sinh viên và ghi lịch sử nếu có.";
            }

            return RedirectToAction("QuanLySinhVien");
        }
        // POST: Xóa sinh viên
        //==============================================================================
        public IActionResult XoaSinhVien(int id)
        {
            var sv = _context.SinhVien.Find(id);
            if (sv != null)
            {
                var user = _context.NguoiDung.FirstOrDefault(x => x.SinhVienID == sv.SinhVienID);
                if (user != null) _context.NguoiDung.Remove(user);

                _context.SinhVien.Remove(sv);
                _context.SaveChanges();
                TempData["ThongBao"] = "Đã xóa sinh viên và tài khoản.";
            }

            return RedirectToAction("QuanLySinhVien");
        }
        // GET: Quản lý nhân viên
        //==============================================================================
        public IActionResult QuanLyNhanVien()
        {
            var danhSach = _context.NhanVien
                .Include(x => x.Khoa)
                .ToList();
            ViewBag.Khoas = _context.Khoa.ToList();
            return View(danhSach);
        }
        // POST: Thêm nhân viên
        //==============================================================================
        [HttpPost]
        public IActionResult ThemNhanVien(string MaNV, string HoTen, string Email, int KhoaID, string ChucVu, string password)
        {
            var nv = new NhanVien
            {
                MaNV = MaNV,
                HoTen = HoTen,
                Email = Email,
                KhoaID = KhoaID,
                ChucVu = ChucVu
            };
            _context.NhanVien.Add(nv);
            _context.SaveChanges();

            var user = new NguoiDung
            {
                Username = MaNV,
                PasswordHash = string.IsNullOrEmpty(password) ? "0000" : password,
                VaiTro = ChucVu, // GVCN, HoiDong, Admin
                NhanVienID = nv.NhanVienID
            };
            _context.NguoiDung.Add(user);
            _context.SaveChanges();

            TempData["ThongBao"] = "Đã thêm nhân viên và tạo tài khoản.";
            return RedirectToAction("QuanLyNhanVien");
        }
        // POST: Sửa nhân viên
        //==============================================================================
        [HttpPost]
        public IActionResult SuaNhanVien(int NhanVienID, string HoTen, string Email, int KhoaID, string ChucVu)
        {
            var nv = _context.NhanVien.Find(NhanVienID);
            if (nv != null)
            {
                nv.HoTen = HoTen;
                nv.Email = Email;
                nv.KhoaID = KhoaID;
                nv.ChucVu = ChucVu;
                

                // Cập nhật luôn vai trò trong bảng người dùng
                var nd = _context.NguoiDung.FirstOrDefault(x => x.NhanVienID == NhanVienID);
                if (nd != null)
                {
                    nd.VaiTro = ChucVu;
                    _context.SaveChanges();
                }

                TempData["ThongBao"] = "Cập nhật thông tin thành công.";
            }

            return RedirectToAction("QuanLyNhanVien");
        }
        // POST: Xóa nhân viên
        //==============================================================================
        public IActionResult XoaNhanVien(int id)
        {
            var nv = _context.NhanVien.Find(id);
            if (nv != null)
            {
                var nd = _context.NguoiDung.FirstOrDefault(x => x.NhanVienID == nv.NhanVienID);
                if (nd != null) _context.NguoiDung.Remove(nd);

                _context.NhanVien.Remove(nv);
                _context.SaveChanges();
                TempData["ThongBao"] = "Đã xóa nhân viên và tài khoản.";
            }

            return RedirectToAction("QuanLyNhanVien");
        }
        // Controllers/AdminController.cs - Quản lý cấu hình hệ thống
        //=============================================================================
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

        // Controllers/AdminController.cs – Thêm phần quản lý trường
        //===========================================================================
        // GET: Quản lý Trường
        public IActionResult QuanLyTruong()
        {
            var ds = _context.Truong.ToList();
            return View(ds);
        }

        [HttpPost]
        public IActionResult ThemTruong(string TenTruong, string DiaChi, string LogoUrl)
        {
            var truong = new Truong { TenTruong = TenTruong, DiaChi = DiaChi, LogoUrl = LogoUrl };
            _context.Truong.Add(truong);
            _context.SaveChanges();
            return RedirectToAction("QuanLyTruong");
        }

        [HttpPost]
        public IActionResult SuaTruong(int TruongID, string TenTruong, string DiaChi, string LogoUrl)
        {
            var truong = _context.Truong.Find(TruongID);
            if (truong != null)
            {
                truong.TenTruong = TenTruong;
                truong.DiaChi = DiaChi;
                truong.LogoUrl = LogoUrl;
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyTruong");
        }

        public IActionResult XoaTruong(int id)
        {
            var truong = _context.Truong.Find(id);
            if (truong != null)
            {
                _context.Truong.Remove(truong);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyTruong");
        }

        // Controllers/AdminController.cs – Thêm phần quản lý khoa
        //===========================================================================
        // GET: Quản lý Khoa
        public IActionResult QuanLyKhoa()
        {
            ViewBag.DanhSachTruong = _context.Truong.ToList();
            var dsKhoa = _context.Khoa.Include(k => k.Truong).ToList();
            return View(dsKhoa);
        }

        [HttpPost]
        public IActionResult ThemKhoa(string TenKhoa, int TruongID, string DiaChi)
        {
            var khoa = new Khoa { TenKhoa = TenKhoa, TruongID = TruongID, DiaChi = DiaChi };
            _context.Khoa.Add(khoa);
            _context.SaveChanges();
            return RedirectToAction("QuanLyKhoa");
        }

        [HttpPost]
        public IActionResult SuaKhoa(int KhoaID, string TenKhoa, int TruongID, string DiaChi)
        {
            var khoa = _context.Khoa.Find(KhoaID);
            if (khoa != null)
            {
                khoa.TenKhoa = TenKhoa;
                khoa.TruongID = TruongID;
                khoa.DiaChi = DiaChi;
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyKhoa");
        }

        public IActionResult XoaKhoa(int id)
        {
            var khoa = _context.Khoa.Find(id);
            if (khoa != null)
            {
                _context.Khoa.Remove(khoa);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyKhoa");
        }


        // Controllers/AdminController.cs – Thêm phần quản lý lớp
        //===========================================================================
        // GET: Quản lý Lớp
        public IActionResult QuanLyLop()
        {
            ViewBag.Khoa = _context.Khoa.Include(k => k.Truong).ToList();
            ViewBag.NienKhoa = _context.NienKhoa.ToList();
            var danhSach = _context.Lop.Include(l => l.Khoa).Include(l => l.NienKhoa).ToList();
            return View(danhSach);
        }

        [HttpPost]
        public IActionResult ThemLop(string TenLop, int KhoaID, int NienKhoaID)
        {
            var lop = new Lop { TenLop = TenLop, KhoaID = KhoaID, NienKhoaID = NienKhoaID };
            _context.Lop.Add(lop);
            _context.SaveChanges();
            return RedirectToAction("QuanLyLop");
        }

        [HttpPost]
        public IActionResult SuaLop(int LopID, string TenLop, int KhoaID, int NienKhoaID)
        {
            var lop = _context.Lop.Find(LopID);
            if (lop != null)
            {
                lop.TenLop = TenLop;
                lop.KhoaID = KhoaID;
                lop.NienKhoaID = NienKhoaID;
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyLop");
        }

        public IActionResult XoaLop(int id)
        {
            var lop = _context.Lop.Find(id);
            if (lop != null)
            {
                _context.Lop.Remove(lop);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyLop");
        }


        // Controllers/AdminController.cs – Thêm phần quản lý niên khóa
        //===========================================================================
        public IActionResult QuanLyNienKhoa()
        {
            var danhSach = _context.NienKhoa.OrderByDescending(nk => nk.NamBatDau).ToList();
            return View(danhSach);
        }

        [HttpPost]
        public IActionResult ThemNienKhoa(string TenNienKhoa, int NamBatDau, int NamKetThuc)
        {
            var nk = new NienKhoa { TenNienKhoa = TenNienKhoa, NamBatDau = NamBatDau, NamKetThuc = NamKetThuc };
            _context.NienKhoa.Add(nk);
            _context.SaveChanges();
            return RedirectToAction("QuanLyNienKhoa");
        }

        [HttpPost]
        public IActionResult SuaNienKhoa(int NienKhoaID, string TenNienKhoa, int NamBatDau, int NamKetThuc)
        {
            var nk = _context.NienKhoa.Find(NienKhoaID);
            if (nk != null)
            {
                nk.TenNienKhoa = TenNienKhoa;
                nk.NamBatDau = NamBatDau;
                nk.NamKetThuc = NamKetThuc;
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyNienKhoa");
        }

        public IActionResult XoaNienKhoa(int id)
        {
            var nk = _context.NienKhoa.Find(id);
            if (nk != null)
            {
                _context.NienKhoa.Remove(nk);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyNienKhoa");
        }


        // Controllers/AdminController.cs – Thêm phần quản lý học ký
        //===========================================================================
        public IActionResult QuanLyHocKy()
        {
            var danhSach = _context.HocKy.OrderByDescending(h => h.NgayBatDau).ToList();
            return View(danhSach);
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
            return RedirectToAction("QuanLyHocKy");
        }

        [HttpPost]
        public IActionResult SuaHocKy(int HocKyID, string TenHocKy, string NamHoc, DateTime? NgayBatDau, DateTime? NgayKetThuc)
        {
            var hk = _context.HocKy.Find(HocKyID);
            if (hk != null)
            {
                hk.TenHocKy = TenHocKy;
                hk.NamHoc = NamHoc;
                hk.NgayBatDau = NgayBatDau;
                hk.NgayKetThuc = NgayKetThuc;
                _context.SaveChanges();
            }
            TempData["ThongBao"] = "Đã cập nhật học kỳ thành công.";
            return RedirectToAction("QuanLyHocKy");
        }

        public IActionResult XoaHocKy(int id)
        {
            var hk = _context.HocKy.Find(id);
            if (hk != null)
            {
                _context.HocKy.Remove(hk);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyHocKy");
        }


        // Controllers/AdminController.cs – Thêm phần quản lý trạng thái học tập
        //===========================================================================
        // GET: Quản lý trạng thái học tập
        public IActionResult QuanLyTrangThaiHocTap()
        {
            var list_trangthaihoctap = _context.CauHinhTrangThaiHocTap.ToList();
            return View(list_trangthaihoctap);
        }
        [HttpPost]
        public IActionResult ThemTrangThaiHocTap(string TenTrangThai, string MoTa)
        {
            var trangthaihoctap = new CauHinhTrangThaiHocTap { TenTrangThai = TenTrangThai, MoTa = MoTa };
            _context.CauHinhTrangThaiHocTap.Add(trangthaihoctap);
            _context.SaveChanges();
            return RedirectToAction("QuanLyTrangThaiHocTap");
        }
        [HttpPost]
        public IActionResult SuaTrangThaiHocTap(int TrangThaiID, string TenTrangThai, string MoTa)
        {
            var trangthaihoctap = _context.CauHinhTrangThaiHocTap.Find(TrangThaiID);
            if (trangthaihoctap != null)
            {
                trangthaihoctap.TenTrangThai = TenTrangThai;
                trangthaihoctap.MoTa = MoTa;
                _context.CauHinhTrangThaiHocTap.Update(trangthaihoctap);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyTrangThaiHocTap");
        }
        public IActionResult XoaTrangThaiHocTap(int id)
        {
            var trangthaihoctap = _context.CauHinhTrangThaiHocTap.Find(id);
            if (trangthaihoctap != null)
            {
                _context.CauHinhTrangThaiHocTap.Remove(trangthaihoctap);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyTrangThaiHocTap");
        }

        // Controllers/AdminController.cs – Thêm phần quản lý vai trò
        //===========================================================================
        public IActionResult QuanLyVaiTro()
        {
            var danhSach = _context.CauHinhVaiTro.ToList();
            return View(danhSach);
        }

        [HttpPost]
        public IActionResult ThemVaiTro(string TenVaiTro, string MoTa)
        {
            var vt = new CauHinhVaiTro { TenVaiTro = TenVaiTro, MoTa = MoTa };
            _context.CauHinhVaiTro.Add(vt);
            _context.SaveChanges();
            return RedirectToAction("QuanLyVaiTro");
        }

        [HttpPost]
        public IActionResult SuaVaiTro(int VaiTroID, string TenVaiTro, string MoTa)
        {
            var vt = _context.CauHinhVaiTro.Find(VaiTroID);
            if (vt != null)
            {
                vt.TenVaiTro = TenVaiTro;
                vt.MoTa = MoTa;
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyVaiTro");
        }

        public IActionResult XoaVaiTro(int id)
        {
            var vt = _context.CauHinhVaiTro.Find(id);
            if (vt != null)
            {
                _context.CauHinhVaiTro.Remove(vt);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyVaiTro");
        }

        // Controllers/AdminController.cs – Thêm phần quản lý xếp loại
        //===========================================================================
        public IActionResult QuanLyXepLoai()
        {
            var danhSach = _context.CauHinhXepLoai.OrderBy(x => x.DiemToiThieu).ToList();
            return View(danhSach);
        }
        
        [HttpPost]
        public IActionResult ThemXepLoai(string TenXepLoai, int DiemToiThieu, int DiemToiDa, string MoTa)
        {
            var xeploai = new CauHinhXepLoai
            {
                TenXepLoai = TenXepLoai,
                DiemToiThieu = DiemToiThieu,
                DiemToiDa = DiemToiDa,
                MoTa = MoTa
            };
            _context.CauHinhXepLoai.Add(xeploai);
            _context.SaveChanges();
            return RedirectToAction("QuanLyXepLoai");
        }
        [HttpPost]
        public IActionResult SuaXepLoai(int XepLoaiID, string TenXepLoai, int DiemToiThieu , int DiemToiDa, string MoTa)
        {
            var xeploai = _context.CauHinhXepLoai.Find(XepLoaiID);
            if (xeploai != null)
            {
                xeploai.TenXepLoai = TenXepLoai;
                xeploai.DiemToiThieu = DiemToiThieu;
                xeploai.DiemToiDa = DiemToiDa;
                xeploai.MoTa = MoTa;
                _context.CauHinhXepLoai.Update(xeploai);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyXepLoai");
        }

        public IActionResult XoaXepLoai(int id)
        {
            var xepLoai = _context.CauHinhXepLoai.Find(id);
            if (xepLoai != null) 
            { 
                _context.CauHinhXepLoai.Remove(xepLoai); 
                _context.SaveChanges(); 
            }
            return RedirectToAction("QuanLyXepLoai");
        }

        // Controllers/AdminController.cs – Thêm phần quản lý nhóm tiêu chí và tiêu chí
        //==========================================================================
        public IActionResult QuanLyNhomTieuChi()
        {
            var danhSach = _context.NhomTieuChi.OrderBy(n => n.TenNhom).ToList();
            return View(danhSach);
        }
        
        [HttpPost]
        public IActionResult ThemNhomTieuChi(string TenNhom, int DiemToiDa, string MoTa)
        {
            var nhom = new NhomTieuChi 
            { 
                TenNhom = TenNhom, 
                DiemToiDa = DiemToiDa, 
                MoTa = MoTa 
            };
            _context.NhomTieuChi.Add(nhom);
            _context.SaveChanges();
            return RedirectToAction("QuanLyNhomTieuChi");
        }

        // POST: Sửa nhóm tiêu chí
        [HttpPost]
        public IActionResult SuaNhomTieuChi(int NhomTieuChiID, string TenNhom, int DiemToiDa, string MoTa)
        {
            var nhom = _context.NhomTieuChi.Find(NhomTieuChiID);
            if (nhom != null)
            {
                nhom.TenNhom = TenNhom;
                nhom.DiemToiDa = DiemToiDa;
                nhom.MoTa = MoTa;
                _context.SaveChanges();
            }
            //_context.NhomTieuChi.Update(nhom!);
            //_context.SaveChanges();
            return RedirectToAction("QuanLyNhomTieuChi");
        }
        // POST: Xóa nhóm tiêu chí
        public IActionResult XoaNhomTieuChi(int id)
        {
            var nhom = _context.NhomTieuChi.Find(id);
            if (nhom != null)
            {
                _context.NhomTieuChi.Remove(nhom);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyNhomTieuChi");
        }

        // Controllers/AdminController.cs – Thêm phần quản lý tiêu chí
        //==========================================================================
        public async Task<IActionResult> QuanLyTieuChi()
        {
            var danhSachTieuChi = await _context.TieuChi.Include(tc => tc.NhomTieuChi).ToListAsync();

            var nhoms = await _context.NhomTieuChi.ToListAsync();
            ViewBag.NhomTieuChi = nhoms;
            return View(danhSachTieuChi);
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
            return RedirectToAction("QuanLyTieuChi");
        }
        [HttpPost]
        public IActionResult SuaTieuChi(int TieuChiID, string TenTieuChi, int DiemToiDa, int NhomTieuChiID, bool YeuCauMinhChung)
        {
            var tc = _context.TieuChi.Find(TieuChiID);
            if (tc != null)
            {
                tc.TenTieuChi = TenTieuChi;
                tc.DiemToiDa = DiemToiDa;
                tc.NhomTieuChiID = NhomTieuChiID;
                tc.YeuCauMinhChung = YeuCauMinhChung;

                _context.TieuChi.Update(tc);
                _context.SaveChanges();
            }
            TempData["ThongBao"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("QuanLyTieuChi");
        }

        public async Task<IActionResult> XoaTieuChi(int id)
        {
            var tc = await _context.TieuChi.FindAsync(id);
            if (tc != null)
            {
                _context.TieuChi.Remove(tc);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("QuanLyTieuChi");
        }

        // Controllers/AdminController.cs – Thêm phần quản lý lịch sử trạng thái 
        //===========================================================================
        public IActionResult QuanLyLichSuTrangThai()
        {
            var ds = _context.LichSuTrangThai.ToList();
            return View(ds);
        }

        //Post: Thêm lịch sử trạng thái
        [HttpPost]
        public IActionResult ThemLichSuTrangThai(int LichSuID, int SinhVienID, string TrangThai, DateTime NgayCapNhat, string GhiChu)
        {
            var lichSu = new LichSuTrangThai
            {
                LichSuID = LichSuID,
                SinhVienID = SinhVienID,
                TrangThai = TrangThai,
                NgayCapNhat = NgayCapNhat,
                GhiChu = GhiChu
            };
            _context.LichSuTrangThai.Add(lichSu);
            _context.SaveChanges();
            return RedirectToAction("QuanLyLichSuTrangThai");
        }
        //Post: Sửa lịch sử trạng thái
        [HttpPost]
        public IActionResult SuaLichSuTrangThai(int id, string TenTrangThai, string MoTa)
        {
            var lichSu = _context.LichSuTrangThai.Find(id);
            if (lichSu != null)
            {
                lichSu.TrangThai = TenTrangThai;
                lichSu.GhiChu = MoTa;
                lichSu.NgayCapNhat = DateTime.Now;
                _context.LichSuTrangThai.Update(lichSu);
                _context.SaveChanges();
            }
            TempData["ThongBao"] = "Cập nhật thông tin thành công.";
            return RedirectToAction("QuanLyLichSuTrangThai");
        }
        //Post: Xóa lịch sử trạng thái
        public IActionResult XoaLichSuTrangThai(int id)
        {
            var lichSu = _context.LichSuTrangThai.Find(id);
            if (lichSu != null)
            {
                _context.LichSuTrangThai.Remove(lichSu);
                _context.SaveChanges();
            }
            return RedirectToAction("QuanLyLichSuTrangThai");
        }

        // Controllers/AdminController.cs – Thêm phần thống kê
        //===========================================================================
        public IActionResult ThongKe()
        {
            ViewBag.NienKhoaList = _context.NienKhoa.OrderByDescending(n => n.NamBatDau).ToList();
            ViewBag.HocKyList = _context.HocKy.OrderByDescending(h => h.NgayBatDau).ToList();
            return View(new List<ThongKeViewModel>()); // mặc định chưa chọn => danh sách trống
        }

        [HttpPost]
        public IActionResult ThongKe(int NienKhoaID, int HocKyID)
        {
            ViewBag.NienKhoaList = _context.NienKhoa.OrderByDescending(n => n.NamBatDau).ToList();
            ViewBag.HocKyList = _context.HocKy.OrderByDescending(h => h.NgayBatDau).ToList();

            var sinhViens = _context.SinhVien.Include(sv => sv.Lop).ThenInclude(l => l!.NienKhoa).ToList();
            var ketQuas = _context.KetQuaRenLuyen.Where(k => k.HocKyID == HocKyID).ToList();

            var danhSach = sinhViens
                .Where(sv => sv.Lop?.NienKhoaID == NienKhoaID)
                .Select(sv => new ThongKeViewModel
                {
                    SinhVien = sv,
                    KetQua = ketQuas.FirstOrDefault(k => k.SinhVienID == sv.SinhVienID)
                })
                .Where(vm => vm.KetQua == null || vm.KetQua.TongDiem < 50)
                .ToList();

            return View(danhSach);
        }





    }
}
