using System.ComponentModel.DataAnnotations;

namespace IT15_TripoleMedelTijol.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string OtpCode { get; set; } // optional
    }
}
