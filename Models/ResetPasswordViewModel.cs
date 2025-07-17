using System.ComponentModel.DataAnnotations;

namespace IT15_TripoleMedelTijol.Models
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
