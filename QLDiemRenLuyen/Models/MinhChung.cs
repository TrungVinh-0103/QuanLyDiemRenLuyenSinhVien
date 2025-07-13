using System;
using System.ComponentModel.DataAnnotations;

namespace QLDiemRenLuyen.Models
{
    public class MinhChung
    {
        [Key]
        public int MinhChungID { get; set; }
        public int PhieuDanhGiaID { get; set; }
        public string? DuongDan { get; set; }
        public string? MoTa { get; set; }
        public DateTime NgayUpload { get; set; } = DateTime.Now;

        public PhieuDanhGia? PhieuDanhGia { get; set; }
    }
}
