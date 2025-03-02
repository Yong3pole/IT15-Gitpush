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
        public string? UserId { get; set; } // FK to AspNetUsers.Id (Nullable)

        // Personal Information
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        [Required]
        [RegularExpression("Male|Female", ErrorMessage = "Gender must be 'Male' or 'Female'.")]
        public required string Gender { get; set; }

        // Contact Information
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        // Address Details
        [StringLength(100)]
        public string Address { get; set; } = string.Empty;

        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [StringLength(50)]
        public string Province { get; set; } = string.Empty;

        [RegularExpression(@"^\d{5}(-\d{4})?$")]
        public string ZipCode { get; set; } = string.Empty;

        // Job Details
        [ForeignKey("JobTitle")]
        public int JobTitleId { get; set; }

        public JobTitle JobTitle { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; }

        public string? ResumePath { get; set; }


        public DateTime DateHired { get; set; } = DateTime.Now;

        public bool EmploymentStatus { get; set; }

        public DateTime? ResignationDate { get; set; }

        // Emergency Contact
        [StringLength(50)]
        public string EmergencyContactName { get; set; } = string.Empty;

        [StringLength(15)]
        public string EmergencyContactPhone { get; set; } = string.Empty;

        [StringLength(50)]
        public string EmergencyContactRelation { get; set; } = string.Empty;

        // Navigation property for Salary
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();

        public string FullName => $"{FirstName} {LastName}";
    }
}