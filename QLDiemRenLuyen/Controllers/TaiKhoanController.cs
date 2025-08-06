using Microsoft.AspNetCore.Mvc;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.Models.CauHinh;
using System.Linq;

namespace QLDiemRenLuyen.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaiKhoanController> _logger;

        // Corrected RoleRedirects dictionary to match CauHinhVaiTro
        private static readonly Dictionary<int, (string Controller, string Action)> RoleRedirects = new()
        {
            { 1, ("SinhVien", "Index") }, // Sinh Viên
            { 2, ("GiaoVien", "Index") }, // Chủ Nhiệm
            { 3, ("HoiDong", "Index") },  // Hội Đồng
            { 4, ("Admin", "Index") }     // Admin
        };

        public TaiKhoanController(ApplicationDbContext context, ILogger<TaiKhoanController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /TaiKhoan/DangNhap
        public IActionResult DangNhap()
        {
            ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
            return View();
        }

        // POST: /TaiKhoan/DangNhap
        //[HttpPost]
        //public IActionResult DangNhap(string username, string password, int vaiTro)
        //{
        //    _logger.LogInformation($"Đăng nhập: username={username}, vaiTro={vaiTro}");

        //    // Validate input
        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        ViewBag.Loi = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
        //        ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
        //        _logger.LogWarning("Đăng nhập thất bại: Thiếu username hoặc password.");
        //        return View();
        //    }

        //    // Check if the role exists
        //    var vaiTroExists = _context.CauHinhVaiTro.Any(v => v.VaiTroID == vaiTro);
        //    if (!vaiTroExists)
        //    {
        //        ViewBag.Loi = "Vai trò được chọn không hợp lệ.";
        //        ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
        //        _logger.LogWarning($"Đăng nhập thất bại: VaiTroID={vaiTro} không tồn tại.");
        //        return View();
        //    }

        //    // Find user and validate password and role
        //    var nguoiDung = _context.NguoiDung
        //        .FirstOrDefault(x => x.Username == username && x.PasswordHash == password && x.VaiTroID == vaiTro);

        //    if (nguoiDung == null)
        //    {
        //        ViewBag.Loi = "Tên đăng nhập, mật khẩu hoặc vai trò không đúng.";
        //        ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
        //        _logger.LogWarning($"Đăng nhập thất bại: Không tìm thấy người dùng với username={username}, vaiTro={vaiTro}.");
        //        return View();
        //    }

        //    _logger.LogInformation($"Đăng nhập thành công: NguoiDungID={nguoiDung.NguoiDungID}, VaiTroID={nguoiDung.VaiTroID}");

        //    // Store session data
        //    HttpContext.Session.SetInt32("NguoiDungID", nguoiDung.NguoiDungID);
        //    HttpContext.Session.SetInt32("VaiTroID", nguoiDung.VaiTroID);
        //    HttpContext.Session.SetString("Username", nguoiDung.Username!);

        //    // Store role-specific IDs
        //    if (nguoiDung.VaiTroID == 1) // Sinh Viên
        //    {
        //        HttpContext.Session.SetInt32("SinhVienID", nguoiDung.SinhVienID ?? 0);
        //    }
        //    else if (nguoiDung.VaiTroID == 2 || nguoiDung.VaiTroID == 3 || nguoiDung.VaiTroID == 4) // Chủ Nhiệm, Hội Đồng, Admin
        //    {
        //        HttpContext.Session.SetInt32("NhanVienID", nguoiDung.NhanVienID ?? 0);
        //    }

        //    // Update last login
        //    nguoiDung.LastLogin = DateTime.Now;
        //    _context.SaveChanges();

        //    // Redirect based on VaiTroID
        //    if (RoleRedirects.TryGetValue(nguoiDung.VaiTroID, out var redirect))
        //    {
        //        _logger.LogInformation($"Chuyển hướng đến {redirect.Controller}/{redirect.Action}");
        //        return RedirectToAction(redirect.Action, redirect.Controller);
        //    }

        //    ViewBag.Loi = "Vai trò không được hỗ trợ trong hệ thống.";
        //    ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
        //    _logger.LogError($"Đăng nhập thất bại: VaiTroID={nguoiDung.VaiTroID} không có trong RoleRedirects.");
        //    return View();
        //}
        [HttpPost]
        public IActionResult DangNhap(string username, string password, int vaiTro)
        {
            _logger.LogInformation($"Đăng nhập: username={username}, vaiTro={vaiTro}");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Loi = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
                _logger.LogWarning("Đăng nhập thất bại: Thiếu username hoặc password.");
                return View();
            }

            var vaiTroExists = _context.CauHinhVaiTro.Any(v => v.VaiTroID == vaiTro);
            if (!vaiTroExists)
            {
                ViewBag.Loi = "Vai trò được chọn không hợp lệ.";
                ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
                _logger.LogWarning($"Đăng nhập thất bại: VaiTroID={vaiTro} không tồn tại.");
                return View();
            }

            var nguoiDung = _context.NguoiDung
                .FirstOrDefault(x => x.Username == username && x.VaiTroID == vaiTro);

            if (nguoiDung == null )
            {
                ViewBag.Loi = "Tên đăng nhập, mật khẩu hoặc vai trò không đúng.";
                ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
                _logger.LogWarning($"Đăng nhập thất bại: Không tìm thấy người dùng hoặc mật khẩu sai với username={username}, vaiTro={vaiTro}.");
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, nguoiDung.PasswordHash))
            {
                ViewBag.Loi = "Mật khẩu không đúng.";
                ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
                _logger.LogWarning($"Đăng nhập thất bại: Mật khóa sai với username={username}, vaiTro={vaiTro}.");
                return View();
            }

            _logger.LogInformation($"Đăng nhập thành công: NguoiDungID={nguoiDung.NguoiDungID}, VaiTroID={nguoiDung.VaiTroID}");

            HttpContext.Session.SetInt32("NguoiDungID", nguoiDung.NguoiDungID);
            HttpContext.Session.SetInt32("VaiTroID", nguoiDung.VaiTroID);
            HttpContext.Session.SetString("Username", nguoiDung.Username!);
           // HttpContext.Session.SetInt32("NhanVienID", nhanVien.NhanVienID);


            if (nguoiDung.VaiTroID == 1)
            {
                HttpContext.Session.SetInt32("SinhVienID", nguoiDung.SinhVienID ?? 0);
            }
            else if (nguoiDung.VaiTroID == 2 || nguoiDung.VaiTroID == 3 || nguoiDung.VaiTroID == 4)
            {
                HttpContext.Session.SetInt32("NhanVienID", nguoiDung.NhanVienID ?? 0);
            }

            nguoiDung.LastLogin = DateTime.Now;
            _context.SaveChanges();

            if (RoleRedirects.TryGetValue(nguoiDung.VaiTroID, out var redirect))
            {
                _logger.LogInformation($"Chuyển hướng đến {redirect.Controller}/{redirect.Action}");
                return RedirectToAction(redirect.Action, redirect.Controller);
            }

            ViewBag.Loi = "Vai trò không được hỗ trợ trong hệ thống.";
            ViewBag.DanhSachVaiTro = _context.CauHinhVaiTro.ToList();
            _logger.LogError($"Đăng nhập thất bại: VaiTroID={nguoiDung.VaiTroID} không có trong RoleRedirects.");
            return View();
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("DangNhap");
        }
    }
}