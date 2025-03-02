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

        // Name

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}"; // Computed Property for Full Name

        [Required]
        [RegularExpression("Male|Female", ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public required string Gender { get; set; } // Gender, Male or Female
        public DateTime? DateOfBirth { get; set; } // Birthday

        // Address Details
        [StringLength(100)]
        public string? HouseNumber { get; set; } = string.Empty; // Can be null because of crazy housings in the Philippines

        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [StringLength(100)]
        public string Barangay { get; set; } = string.Empty;

        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [StringLength(50)]
        public string Province { get; set; } = string.Empty;

        [RegularExpression(@"^\d{4}$", ErrorMessage = "Zip Code must be 4 digits.")]
        public string ZipCode { get; set; } = string.Empty;

        public string CompleteAddress =>
            $"{(string.IsNullOrWhiteSpace(HouseNumber) ? "" : HouseNumber + " ")}{Street}, {Barangay}, {City}, {Province} {ZipCode}".Trim();


        // Contact Details

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(09\d{9}|\+63\d{10})$", ErrorMessage = "Phone number must be in the format 09XXXXXXXXX or +63XXXXXXXXXX.")]
        public string Phone { get; set; } = string.Empty;

        // Resume Path
        public string? ResumePath { get; set; }

        [Required]
        [RegularExpression("For Review|Hired|Rejected", ErrorMessage = "Status must be 'For Review', 'Hired', or 'Rejected'.")]
        public string Status { get; set; } = "For Review"; // Application Status

        public DateTime DateApplied { get; set; } = DateTime.Now;

        // Navigation Property
        public JobPosting? JobPosting { get; set; }
    }
}