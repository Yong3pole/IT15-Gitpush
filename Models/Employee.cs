using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    public class Employee
    {
        [Key]
        public string EmployeeID { get; set; } = Guid.NewGuid().ToString(); // Auto-generate ID

        [ForeignKey("Applicant")]
        public int? ApplicantID { get; set; } // FK to Applicant (Nullable for direct hires)

        public Applicant? Applicant { get; set; } // Navigation Property

        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; } // FK to AspNetUsers.Id (Nullable), only selected Job Titles will be given access to HRMS

        // Personal Information
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [RegularExpression("Male|Female", ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public required string Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Contact Information
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(09\d{9}|\+63\d{10})$", ErrorMessage = "Phone number must be in the format 09XXXXXXXXX or +63XXXXXXXXXX.")]
        public string Phone { get; set; } = string.Empty;

        // Address Details
        [StringLength(100)]
        public string? HouseNumber { get; set; } = string.Empty;

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

        // Job Details
        [ForeignKey("JobTitle")]
        public int? JobTitleId { get; set; }

        public required JobTitle JobTitle { get; set; }

        [ForeignKey("Department")]
        public int? DepartmentId { get; set; }

        public required Department Department { get; set; }

        public string? ResumePath { get; set; }

        public DateTime DateHired { get; set; } = DateTime.Now;

        public bool EmploymentStatus { get; set; }

        public DateTime? ResignationDate { get; set; }

        // Emergency Contact
        [StringLength(50)]
        public string EmergencyContactName { get; set; } = string.Empty;

        [StringLength(11)]
        public string EmergencyContactPhone { get; set; } = string.Empty;

        [StringLength(50)]
        public string EmergencyContactRelation { get; set; } = string.Empty;

        // Navigation property for Salary
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
    }
}
