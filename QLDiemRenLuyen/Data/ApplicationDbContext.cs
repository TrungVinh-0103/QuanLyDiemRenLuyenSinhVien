using Microsoft.EntityFrameworkCore;
using QLDiemRenLuyen.Models;
using QLDiemRenLuyen.Models.CauHinh;

namespace QLDiemRenLuyen.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<NguoiDung> NguoiDung { get; set; }
        public DbSet<ChucVu> ChucVu { get; set; }
        public DbSet<ChuNhiem> ChuNhiem { get; set; }
        public DbSet<SinhVien> SinhVien { get; set; }
        public DbSet<NhanVien> NhanVien { get; set; }
        public DbSet<Truong> Truong { get; set; }
        public DbSet<Khoa> Khoa { get; set; }
        public DbSet<Lop> Lop { get; set; }
        public DbSet<NienKhoa> NienKhoa { get; set; }
        public DbSet<HocKy> HocKy { get; set; }
        public DbSet<NhomTieuChi> NhomTieuChi { get; set; }
        public DbSet<TieuChi> TieuChi { get; set; }
        public DbSet<PhieuDanhGia> PhieuDanhGia { get; set; }
        public DbSet<ChiTietPhieuDanhGia> ChiTietPhieuDanhGia { get; set; }
        public DbSet<KetQuaRenLuyen> KetQuaRenLuyen { get; set; }
        public DbSet<CauHinhTrangThaiSinhVien> CauHinhTrangThaiSinhVien { get; set; }
        public DbSet<CauHinhTrangThaiDanhGia> CauHinhTrangThaiDanhGia { get; set; }
        public DbSet<CauHinhVaiTro> CauHinhVaiTro { get; set; }
        public DbSet<CauHinhXepLoai> CauHinhXepLoai { get; set; }
        public DbSet<TrangThaiSinhVien> TrangThaiSinhVien { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
