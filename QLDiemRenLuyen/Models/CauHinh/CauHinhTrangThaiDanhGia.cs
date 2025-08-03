using System.ComponentModel.DataAnnotations;


namespace QLDiemRenLuyen.Models.CauHinh
{
        public class CauHinhTrangThaiDanhGia
        {
            [Key]
            public int TrangThaiDanhGiaID { get; set; }

            [Required(ErrorMessage = "Tên trạng thái đánh giá không được để trống")]
            [MaxLength(50)]
            public string? TenTrangThai { get; set; }
        }
}