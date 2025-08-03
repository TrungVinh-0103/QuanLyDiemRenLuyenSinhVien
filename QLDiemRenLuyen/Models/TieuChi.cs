using System.ComponentModel.DataAnnotations;

public class TieuChi
{
    public int TieuChiID { get; set; }

    [Required]
    public int NhomTieuChiID { get; set; }

    [Required]
    [Display(Name = "Tên tiêu chí")]
    public string? TenTieuChi { get; set; }

    [Required]
    public int DiemToiDa { get; set; }


    public NhomTieuChi? NhomTieuChi { get; set; }
}
