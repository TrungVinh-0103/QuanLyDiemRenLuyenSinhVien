using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.Models.CauHinh;
using QLDiemRenLuyen.ViewModels;
using System.Linq;

namespace QLDiemRenLuyen.Controllers
{
    public class GiaoVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiaoVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Giả định có session chứa NhanVienID sau khi đăng nhập
        private int GetNhanVienID()
        {
            return HttpContext.Session.GetInt32("NhanVienID") ?? 0;
        }
        // Trang chính GVCN
        public IActionResult Index()
        {
            int nhanVienID = GetNhanVienID();
            var giaoVien = _context.NhanVien.Include(k => k.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            ViewBag.GiaoVien = giaoVien;
            return View();
        }

        // Danh sách phiếu cần duyệt
        public IActionResult DanhSachPhieuDanhGia()
        {
            int nhanVienID = GetNhanVienID();

            // Lấy danh sách lớp mà giáo viên đang là GVCN trong học kỳ hiện tại
            var lopIDs = _context.ChuNhiem
                .Where(cn => cn.NhanVienID == nhanVienID)
                .Select(cn => cn.LopID)
                .Distinct()
                .ToList();

            var phieus = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .Where(p => p.TrangThaiDanhGiaID == 2 && p.SinhVien != null)
                .ToList()
                .Where(p => lopIDs.Contains(p.SinhVien!.LopID))
                .ToList();

            return View(phieus);
        }

        // GET: Duyệt phiếu
        public IActionResult DuyetPhieu(int id)
        {
            var phieu = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .FirstOrDefault(p => p.PhieuDanhGiaID == id);

            if (phieu == null)
                return NotFound();

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
                        }).ToList()
                }).ToList();

            var tieuChis = _context.TieuChi
                .Select(t => new TieuChiViewModel
                {
                    TieuChiID = t.TieuChiID,
                    TenTieuChi = t.TenTieuChi,
                    DiemToiDa = t.DiemToiDa
                }).ToList();

            var chiTietPhieu = _context.ChiTietPhieuDanhGia
                .Where(c => c.PhieuDanhGiaID == id)
                .ToList();

            // Tính tổng điểm sinh viên tự đánh giá
            int tongDiemSV = chiTietPhieu.Sum(c => c.DiemTuDanhGia);

            var vm = new TuDanhGiaViewModel
            {
                PhieuDanhGiaID = id,
                SinhVienID = phieu.SinhVienID,
                HocKyID = phieu.HocKyID,
                NhomTieuChi = nhomTieuChis,
                TieuChi = tieuChis,
                ChiTietPhieu = chiTietPhieu,
                TongDiemTuDanhGia = tongDiemSV
            };

            ViewBag.SinhVien = phieu.SinhVien;
            ViewBag.HocKy = phieu.HocKy;

            return View(vm);
        }

        // POST: Duyệt phiếu
        [HttpPost]
        public IActionResult DuyetPhieu(TuDanhGiaViewModel vm, Dictionary<int, int> DiemGiaoVienDeXuat)
        {
            var phieu = _context.PhieuDanhGia.FirstOrDefault(p => p.PhieuDanhGiaID == vm.PhieuDanhGiaID);
            if (phieu == null)
                return NotFound();

            var chitiet = _context.ChiTietPhieuDanhGia
                            .Where(c => c.PhieuDanhGiaID == vm.PhieuDanhGiaID).ToList();

            // Gom nhóm tiêu chí theo nhóm tiêu chí
            var tieuChiList = _context.TieuChi.ToList();
            var nhomTieuChiList = _context.NhomTieuChi.ToDictionary(n => n.NhomTieuChiID, n => n.DiemToiDa);

            // Gom điểm theo nhóm
            var tongTheoNhom = new Dictionary<int, int>();

            foreach (var ct in chitiet)
            {
                if (DiemGiaoVienDeXuat.TryGetValue(ct.TieuChiID, out int diem))
                {
                    ct.DiemGiaoVienDeXuat = diem;

                    var tieuChi = tieuChiList.FirstOrDefault(t => t.TieuChiID == ct.TieuChiID);
                    if (tieuChi != null)
                    {
                        int nhomID = tieuChi.NhomTieuChiID;
                        if (!tongTheoNhom.ContainsKey(nhomID))
                            tongTheoNhom[nhomID] = 0;

                        tongTheoNhom[nhomID] += diem;
                    }
                }
            }

            // Tính tổng điểm GVCN duyệt (có giới hạn theo từng nhóm)
            int tongDiem = 0;
            foreach (var kv in tongTheoNhom)
            {
                int nhomID = kv.Key;
                int diemNhom = kv.Value;
                int gioiHan = nhomTieuChiList.ContainsKey(nhomID) ? nhomTieuChiList[nhomID] : diemNhom;
                tongDiem += Math.Min(diemNhom, gioiHan); // Áp giới hạn ở đây
            }

            // Cập nhật phiếu
            phieu.TongDiemGiaoVienDeXuat = tongDiem;
            phieu.TrangThaiDanhGiaID = 3; // GVCN đã duyệt

            _context.SaveChanges();

            TempData["Success"] = "Đã duyệt phiếu và gửi lên Hội đồng.";
            return RedirectToAction("DanhSachPhieuDanhGia");
        }

        // Lịch sử phiếu đã duyệt
        public IActionResult LichSuPhieuDanhGia()
        {
            int nhanVienID = GetNhanVienID();

            var lopIDs = _context.ChuNhiem
                .Where(cn => cn.NhanVienID == nhanVienID)
                .Select(cn => cn.LopID)
                .Distinct()
                .ToList();

            var ds = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .Where(p => (p.TrangThaiDanhGiaID >= 3 || p.TrangThaiDanhGiaID == 5) && p.SinhVien != null)
                .ToList()
                .Where(p => lopIDs.Contains(p.SinhVien!.LopID))
                .ToList();


            return View(ds);
        }
    }
}
