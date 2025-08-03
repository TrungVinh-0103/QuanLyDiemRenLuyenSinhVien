using QLDiemRenLuyen.Models;
using System.ComponentModel.DataAnnotations;

public class NhomTieuChi
{
    public int NhomTieuChiID { get; set; }

    [Required]
    [Display(Name = "Tên nhóm tiêu chí")]
    public string? TenNhom { get; set; }

    [Display(Name = "Điểm tối đa")]
    public int DiemToiDa { get; set; }
    
    public ICollection<TieuChi>? TieuChi { get; set; }
}
