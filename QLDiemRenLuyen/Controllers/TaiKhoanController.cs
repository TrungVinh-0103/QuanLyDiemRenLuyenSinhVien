using Microsoft.AspNetCore.Mvc;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class TaiKhoanController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaiKhoanController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult DangNhap()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DangNhap(string username, string password, string vaiTro)
    {
        var nguoiDung = _context.NguoiDung
            .Include(x => x.SinhVien)
            .Include(x => x.NhanVien)
            .FirstOrDefault(x => x.Username == username && x.PasswordHash == password && x.VaiTro == vaiTro);

        if(nguoiDung == null)
        {
            ViewBag.ThongBao = "Thông tin đăng nhập không đúng";
            return View();
        }

        HttpContext.Session.SetString("Username", username);
        HttpContext.Session.SetString("VaiTro", vaiTro);
        HttpContext.Session.SetInt32("NguoiDungID", nguoiDung.NguoiDungID);

        if (nguoiDung.VaiTro == "SinhVien") return RedirectToAction("Index", "SinhVien");

        if (nguoiDung.VaiTro == "GVCN") return RedirectToAction("Index", "GVCN");

        if (nguoiDung.VaiTro == "HoiDong") return RedirectToAction("Index", "HoiDong");

        return RedirectToAction("Index", "Admin");
        //if (nguoiDung != null)
        //{
        //    nguoiDung.LastLogin = DateTime.Now;
        //    _context.SaveChanges();

        //    HttpContext.Session.SetString("Username", username);
        //    HttpContext.Session.SetString("VaiTro", vaiTro);
        //    //HttpContext.Session.SetInt32("NguoiDungID", nguoiDung.NguoiDungID);

        //    return RedirectToAction("Index", vaiTro switch
        //    {
        //        "Admin" => "Admin",
        //        "SinhVien" => "SinhVien",
        //        "GVCN" => "GVCN",
        //        "HoiDong" => "HoiDong",

        //        _ => "TaiKhoan"
        //    });
        //}

        //ViewBag.ThongBao = "Thông tin đăng nhập không đúng";
        //return View();
    }

    public IActionResult DangXuat()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("DangNhap");
    }
}
