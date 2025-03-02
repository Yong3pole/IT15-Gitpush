// Models/Applicant.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    public class Applicant
    {
        [Key]
        public int ApplicantID { get; set; }

        [ForeignKey("JobPosting")]
        public int JobID { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string? ResumePath { get; set; }

        [Required]
        [RegularExpression("For Review|Hired|Rejected", ErrorMessage = "Status must be 'For Review', 'Hired', or 'Rejected'.")]
        public string Status { get; set; } = "For Review";

        public DateTime DateApplied { get; set; }

        // Navigation Property
        public JobPosting? JobPosting { get; set; }

        // Computed Property for Full Name
        public string FullName => $"{FirstName} {LastName}";
    }
}