// Models/JobPosting.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    // Models/JobPosting.cs
    public class JobPosting
    {
        [Key]
        public int JobID { get; set; } // Primary Key

        [ForeignKey("Department")]
        public int DepartmentId { get; set; } // Foreign Key to Department

        [ForeignKey("JobTitle")]
        public int JobTitleId { get; set; } // Foreign Key to JobTitle

        public string? Description { get; set; } // Optional: Job description

        [Required]
        public string SalaryRange { get; set; } // Required: Salary range

        [Required]
        public string Status { get; set; } // Required: Job status (e.g., "Open", "Closed", "Filled")

        public DateTime DatePosted { get; set; } // Date the job was posted

        public int? HiredApplicantID { get; set; } // Nullable: ID of the hired applicant

        // Navigation Properties
        public Department? Department { get; set; } // Reference to Department
        public JobTitle? JobTitle { get; set; } // Reference to JobTitle

        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>(); // List of applicants
    }
}