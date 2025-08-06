using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Data;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.ViewModels;
using OfficeOpenXml.Style;

namespace QLDiemRenLuyen.Controllers
{
    public class HoiDongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoiDongController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Trang chính hội đồng
        public IActionResult Index()
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan"); // hoặc trang lỗi

            var giaoVien = _context.NhanVien
                .Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);

            ViewBag.GiaoVien = giaoVien;

            return View();
        }


        // Danh sách phiếu cần duyệt
        public IActionResult DanhSachPhieu()
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;

            var phieus = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .Where(p => p.TrangThaiDanhGiaID == 3 && /*GVCN đã duyệt*/
                            p.SinhVien!.KhoaID == giaoVien!.KhoaID)
                .ToList();

            return View(phieus);
        }

        // GET: Duyệt phiếu
        public IActionResult DuyetPhieu(int id)
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;

            var phieu = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .FirstOrDefault(p => p.PhieuDanhGiaID == id);

            if (phieu == null)
                return NotFound();

            // Lấy danh sách nhóm tiêu chí và tiêu chí tương ứng
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

            var vm = new TuDanhGiaViewModel
            {
                PhieuDanhGiaID = id,
                SinhVienID = phieu.SinhVienID,
                HocKyID = phieu.HocKyID,
                NhomTieuChi = nhomTieuChis,
                TieuChi = tieuChis,
                ChiTietPhieu = chiTietPhieu
            };

            ViewBag.SinhVien = phieu.SinhVien;
            ViewBag.HocKy = phieu.HocKy;

            return View(vm);
        }

        // POST: Duyệt phiếu
        [HttpPost]
        public async Task<IActionResult> DuyetPhieu(TuDanhGiaViewModel vm, Dictionary<int, int> DiemHoiDongDuyet)
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;

            var phieu = _context.PhieuDanhGia.FirstOrDefault(p => p.PhieuDanhGiaID == vm.PhieuDanhGiaID);
            if (phieu == null) return NotFound();

            var chitiet = _context.ChiTietPhieuDanhGia
                            .Where(c => c.PhieuDanhGiaID == vm.PhieuDanhGiaID).ToList();

            // Lấy danh sách tiêu chí & nhóm tiêu chí
            var tieuChiList = _context.TieuChi.ToList();
            var nhomTieuChiList = _context.NhomTieuChi.ToDictionary(n => n.NhomTieuChiID, n => n.DiemToiDa);

            // ✅ Gom điểm theo từng nhóm tiêu chí
            var tongTheoNhom = new Dictionary<int, int>();

            foreach (var ct in chitiet)
            {
                if (DiemHoiDongDuyet.TryGetValue(ct.TieuChiID, out int diem))
                {
                    ct.DiemHoiDongDuyet = diem;

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

            // ✅ Tổng điểm được tính theo nhóm có giới hạn
            int tongDiem = 0;
            foreach (var kv in tongTheoNhom)
            {
                int nhomID = kv.Key;
                int diemNhom = kv.Value;
                int gioiHan = nhomTieuChiList.ContainsKey(nhomID) ? nhomTieuChiList[nhomID] : diemNhom;

                tongDiem += Math.Min(diemNhom, gioiHan); // Áp dụng giới hạn điểm nhóm
            }

            // Cập nhật phiếu
            phieu.TongDiemHoiDongDuyet = tongDiem;
            phieu.TrangThaiDanhGiaID = 5; // Hội đồng đã duyệt

            await _context.SaveChangesAsync();

            // Tìm xếp loại phù hợp dựa vào tổng điểm
            var xepLoai = _context.CauHinhXepLoai
                .FirstOrDefault(x => tongDiem >= x.DiemToiThieu && tongDiem <= x.DiemToiDa);

            // Nếu tìm thấy, tạo kết quả rèn luyện
            if (xepLoai != null)
            {
                var ketQua = new KetQuaRenLuyen
                {
                    SinhVienID = phieu.SinhVienID,
                    HocKyID = phieu.HocKyID,
                    PhieuDanhGiaID = phieu.PhieuDanhGiaID,
                    TongDiemHoiDongDuyet = tongDiem,
                    XepLoaiID = xepLoai.XepLoaiID,
                    NgayCapNhat = DateTime.Now
                };

                _context.KetQuaRenLuyen.Add(ketQua);
                await _context.SaveChangesAsync(); // ✅ LƯU DỮ LIỆU KẾT QUẢ VÀO DB
            }

            //_context.SaveChanges();
            TempData["Success"] = "Phiếu đã được duyệt thành công!";
            return RedirectToAction("DanhSachPhieu");
        }


        // Lịch sử phiếu đã duyệt
        public IActionResult LichSuPhieu()
        {
            // Lấy danh sách phiếu đã duyệt
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;

            var phieus = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .Where(p => p.TrangThaiDanhGiaID == 5 &&
                            p.TongDiemHoiDongDuyet > 0 &&
                            p.SinhVien!.KhoaID == khoaID) // Lọc theo khoa
                .ToList();

            return View(phieus);
        }

        //Công bố kết quả rên luyện
        public IActionResult CongBo()
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;


            //ViewBag.Lops = _context.Lop
            //    .Include(k => k.Khoa)
            //    .Where(l => l.KhoaID == giaoVien!.KhoaID)
            //    .ToList();
            //ViewBag.HocKys = _context.HocKy
            //    .Include(h => h.NienKhoa)
            //    .Where(h => h.NienKhoa!.NienKhoaID == giaoVien!.NienKhoa!.NienKhoaID)
            //    .ToList();
            //ViewBag.NamHocs = _context.HocKy.Select(h => h.NamHoc).Distinct().ToList();
            //ViewBag.SinhViens = _context.SinhVien
            //    .Include(l => l.Lop)
            //    .Where(sv => sv.KhoaID == giaoVien.KhoaID)
            //    .ToList();
            ViewBag.Lops = _context.Lop
                .Where(l => l.KhoaID == khoaID)
                .ToList();

            ViewBag.SinhViens = _context.SinhVien
                .Where(sv => sv.KhoaID == khoaID)
                .Include(sv => sv.Lop)
                .ToList();

            ViewBag.HocKys = _context.HocKy.Include(h => h.NienKhoa).ToList();
            ViewBag.NamHocs = _context.HocKy.Select(h => h.NamHoc).Distinct().ToList();

            return View();
        }
        [HttpPost]
        public IActionResult XuatKetQua(string loai, int? lopId, int? hocKyId, string? namHoc, string? hoTen, string? maSinhVien, string dinhDang)
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;

            var query = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .Where(p => p.TrangThaiDanhGiaID == 5 && p.SinhVien!.KhoaID == khoaID);

            if (loai == "CaNhan")
            {                      
                if (!string.IsNullOrEmpty(hoTen))
                    query = query.Where(p => p.SinhVien!.HoTen.Contains(hoTen));
                if (!string.IsNullOrEmpty(maSinhVien))
                    query = query.Where(p => p.SinhVien!.MaSV.Contains(maSinhVien));
                if (Request.Form.TryGetValue("hocKyIdCaNhan", out var hocKyIdVal) && int.TryParse(hocKyIdVal, out int hkCaNhan))
                    query = query.Where(p => p.HocKyID == hkCaNhan);
                if (!string.IsNullOrEmpty(namHoc))                        
                    query = query.Where(p => p.HocKy!.NamHoc == namHoc);                                         
            }
            else if (loai == "Lop" && lopId.HasValue)
                query = query.Where(p => p.SinhVien!.LopID == lopId);
            else if (loai == "HocKy" && hocKyId.HasValue)
                query = query.Where(p => p.HocKyID == hocKyId);
            else if (loai == "NamHoc" && !string.IsNullOrEmpty(namHoc))
                query = query.Where(p => p.HocKy!.NamHoc == namHoc);

            var ds = query.ToList();

            if (dinhDang == "PDF")
                return Content("✅ Xuất PDF thành công (mock).");
            else if (dinhDang == "Excel")
                return Content("✅ Xuất Excel thành công (mock).");

            return Content("⚠️ Chưa chọn định dạng.");
        }

        [HttpPost]
        public JsonResult LayKetQuaAjax(string loai, int? lopId, int? hocKyId, string? hoTen, string? maSinhVien)
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return Json(new { error = "Không xác định nhân viên" });

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null)
                return Json(new { error = "Không tìm thấy nhân viên" });

            int khoaID = giaoVien.KhoaID ?? 0;

            var query = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .Where(p => p.TrangThaiDanhGiaID == 5 &&
                            p.SinhVien!.KhoaID == khoaID);

            if (loai == "CaNhan")
            {
                if (!string.IsNullOrEmpty(maSinhVien))
                    query = query.Where(p => p.SinhVien!.MaSV.Contains(maSinhVien));
                if (hocKyId.HasValue)
                    query = query.Where(p => p.HocKyID == hocKyId);
            }
            else if (loai == "Lop" && lopId.HasValue)
                query = query.Where(p => p.SinhVien!.LopID == lopId);
            else if (loai == "HocKy" && hocKyId.HasValue)
                query = query.Where(p => p.HocKyID == hocKyId);

            var ds = query.Select(p => new
            {
                p.TongDiemHoiDongDuyet,
                SinhVien = new
                {
                    p.SinhVien!.HoTen,
                    p.SinhVien.MaSV,
                    Lop = new { p.SinhVien.Lop!.TenLop }
                },
                HocKy = new
                {
                    p.HocKy!.TenHocKy,
                    p.HocKy.NamHoc
                }
            }).ToList();

            return Json(ds);
        }

        public IActionResult KetQuaPartial(List<PhieuDanhGia> ds)
        {
            return PartialView(ds);
        }

        private byte[] TaoFilePDF(List<PhieuDanhGia> ds, out string fileName, string khoGiay)
        {
            fileName = "KetQuaRenLuyen.pdf";
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(khoGiay == "A5" ? PageSizes.A5 : PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("KẾT QUẢ RÈN LUYỆN SINH VIÊN").FontSize(18).Bold().AlignCenter();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); // #
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("STT");
                            header.Cell().Element(CellStyle).Text("Họ tên");
                            header.Cell().Element(CellStyle).Text("Mã SV");
                            header.Cell().Element(CellStyle).Text("Lớp");
                            header.Cell().Element(CellStyle).Text("Học kỳ");
                            header.Cell().Element(CellStyle).Text("Điểm");

                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold()).Padding(2).Background("#EEE");
                        });

                        int index = 1;
                        foreach (var p in ds)
                        {
                            table.Cell().Text(index.ToString()); index++;
                            table.Cell().Text(p.SinhVien?.HoTen ?? "");
                            table.Cell().Text(p.SinhVien?.MaSV ?? "");
                            table.Cell().Text(p.SinhVien?.Lop?.TenLop ?? "");
                            table.Cell().Text($"{p.HocKy?.TenHocKy} - {p.HocKy?.NamHoc}");
                            table.Cell().Text(p.TongDiemHoiDongDuyet.ToString());
                        }
                    });
                });
            });

            using var stream = new MemoryStream();
            doc.GeneratePdf(stream);
            return stream.ToArray();
        }

        private byte[] TaoFileExcel(List<PhieuDanhGia> ds, out string fileName)
        {
            fileName = "KetQuaRenLuyen.xlsx";

            // ✅ EPPlus 5.x sử dụng dòng này để set license miễn phí
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Kết quả");

            sheet.Cells["A1:F1"].Merge = true; 
            sheet.Cells["A1"].Value = "KẾT QUẢ RÈN LUYỆN SINH VIÊN"; 
            sheet.Cells["A1"].Style.Font.Size = 18; 
            sheet.Cells["A1"].Style.Font.Bold = true; 
            sheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            sheet.Cells[2, 1].Value = "STT";
            sheet.Cells[2, 2].Value = "Họ tên";
            sheet.Cells[2, 3].Value = "Mã SV";
            sheet.Cells[2, 4].Value = "Lớp";
            sheet.Cells[2, 5].Value = "Học kỳ";
            sheet.Cells[2, 6].Value = "Điểm";

            int row = 3;
            int index = 1;
            foreach (var p in ds)
            {
                sheet.Cells[row, 1].Value = index++;
                sheet.Cells[row, 2].Value = p.SinhVien?.HoTen;
                sheet.Cells[row, 3].Value = p.SinhVien?.MaSV;
                sheet.Cells[row, 4].Value = p.SinhVien?.Lop?.TenLop;
                sheet.Cells[row, 5].Value = $"{p.HocKy?.TenHocKy} - {p.HocKy?.NamHoc}";
                sheet.Cells[row, 6].Value = p.TongDiemHoiDongDuyet;
                row++;
            }

            return package.GetAsByteArray();
        }


        [HttpPost]
        public IActionResult XuatFile(string dinhDang, string khoGiay,
    string loai, int? lopId, int? hocKyId, string? namHoc, string? maSinhVien)
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;

            var query = _context.PhieuDanhGia
                .Include(p => p.SinhVien).ThenInclude(s => s!.Lop)
                .Include(p => p.HocKy)
                .Where(p => p.TrangThaiDanhGiaID == 5 && p.SinhVien!.KhoaID == khoaID);

            // Áp dụng lọc
            if (loai == "CaNhan")
            {
                if (!string.IsNullOrEmpty(maSinhVien))
                    query = query.Where(p => p.SinhVien!.MaSV.Contains(maSinhVien));
                if (hocKyId.HasValue)
                    query = query.Where(p => p.HocKyID == hocKyId);
            }
            else if (loai == "Lop" && lopId.HasValue)
                query = query.Where(p => p.SinhVien!.LopID == lopId);
            else if (loai == "HocKy" && hocKyId.HasValue)
                query = query.Where(p => p.HocKyID == hocKyId);
            else if (loai == "NamHoc" && !string.IsNullOrEmpty(namHoc))
                query = query.Where(p => p.HocKy!.NamHoc == namHoc);

            var ds = query.ToList();

            if (ds == null || !ds.Any())
                return Content("❌ Không có dữ liệu để xuất!");

            byte[] fileBytes;
            string fileName;
            string contentType;

            if (dinhDang == "PDF")
            {
                fileBytes = TaoFilePDF(ds, out fileName, khoGiay);
                contentType = "application/pdf";
            }
            else
            {
                fileBytes = TaoFileExcel(ds, out fileName);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }

            return File(fileBytes, contentType, fileName);
        }

        // Thong ke chưa đánh giá 
        [HttpGet]
        public IActionResult ThongKeChuaDanhGiaHoiDong(int? HocKyID, int? LopID, string? Loai)
        {
            int? nhanVienID = HttpContext.Session.GetInt32("NhanVienID");
            if (nhanVienID == null)
                return RedirectToAction("Login", "TaiKhoan");

            var giaoVien = _context.NhanVien.Include(g => g.Khoa)
                .FirstOrDefault(g => g.NhanVienID == nhanVienID);
            if (giaoVien == null) return NotFound();

            int khoaID = giaoVien.KhoaID ?? 0;

            ViewBag.HocKys = _context.HocKy.Include(h => h.NienKhoa).ToList();
            ViewBag.Lops = _context.Lop
                .Where(l => l.KhoaID == giaoVien.KhoaID)
                .OrderBy(l => l.TenLop).ToList();
            ViewBag.HocKyID = HocKyID;
            ViewBag.LopID = LopID;
            ViewBag.Loai = Loai;

            // ⚠️ Kiểm tra bắt buộc chọn Học kỳ và Loại
            if (!HocKyID.HasValue || string.IsNullOrEmpty(Loai))
            {
                TempData["Error"] = "Vui lòng chọn đầy đủ Học kỳ và Loại thống kê.";
                return View("ThongKeChuaDanhGiaHoiDong", new List<SinhVien>());
            }

            // ✅ Lấy toàn bộ sinh viên thuộc khoa
            var sinhViens = _context.SinhVien
                .Include(sv => sv.Lop)
                .Include(sv => sv.KetQuaRenLuyen)
                .Where(sv => sv.KhoaID == giaoVien.KhoaID)
                .AsQueryable();

            // ✅ Nếu có chọn Lớp → lọc thêm
            if (LopID.HasValue)
                sinhViens = sinhViens.Where(sv => sv.LopID == LopID);

            List<SinhVien> ketQua = new();

            if (Loai == "ChuaDat")
            {
                // SV có điểm nhưng <= 49
                ketQua = sinhViens
                    .Where(sv => _context.KetQuaRenLuyen.Any(kq =>
                        kq.SinhVienID == sv.SinhVienID &&
                        kq.HocKyID == HocKyID &&
                        kq.TongDiemHoiDongDuyet <= 49))
                    .ToList();
            }
            else if (Loai == "ChuaCoDiem")
            {
                // SV không có kết quả nào trong học kỳ
                ketQua = sinhViens
                    .Where(sv => !_context.KetQuaRenLuyen.Any(kq =>
                        kq.SinhVienID == sv.SinhVienID &&
                        kq.HocKyID == HocKyID))
                    .ToList();
            }

            return View("ThongKeChuaDanhGiaHoiDong", ketQua);
        }


    }
}

