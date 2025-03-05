using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IT15_TripoleMedelTijol.Models
{
    public class LeaveRequest
    {
        [Key]
        public int LeaveRequestId { get; set; }

        [ForeignKey("Employee")]
        public required string EmployeeID { get; set; } // FK to Employee

        public required Employee Employee { get; set; } // Navigation property

        [ForeignKey("LeaveType")]
        public int LeaveTypeId { get; set; } // FK to LeaveType

        public required LeaveType LeaveType { get; set; } // Navigation property

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int TotalDays => (EndDate - StartDate).Days + 1; // Calculate days requested

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public string? Reason { get; set; } // Optional reason for leave

        [ForeignKey("Approver")]
        public string? ApproverId { get; set; } // FK to HR or Manager who approves the leave

        public ApplicationUser? Approver { get; set; } // Navigation property

        public DateTime? ApprovalDate { get; set; }
    }

}
