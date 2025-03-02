using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    public class Performance
    {
        [Key]
        public int PerformanceId { get; set; } // Primary Key

        [Required]
        [ForeignKey("ApplicationUser")]
        public string EmployeeId { get; set; } // FK from AspNetUsers.Id

        public DateTime ReviewDate { get; set; } // Date of performance review
        public string Reviewer { get; set; } // Who reviewed the performance
        public string Comments { get; set; } // Performance feedback
        public int Rating { get; set; } // Rating score (e.g., 1-5)

        public ApplicationUser Employee { get; set; } // Navigation Property
    }
}
