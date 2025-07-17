using System.ComponentModel.DataAnnotations;

public class VerifyResetOtpViewModel
{
    [Required]
    [Display(Name = "OTP")]
    public string Otp { get; set; }
}
