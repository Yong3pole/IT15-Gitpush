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

        public Employee? Employee { get; set; }

        [ForeignKey("LeaveType")]
        public int LeaveTypeId { get; set; } // FK to LeaveType

        public LeaveType? LeaveType { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }


        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        public string? Reason { get; set; } // Optional reason for leave

        [ForeignKey("Approver")]
        public string? ApproverId { get; set; } // FK to HR or Manager who approves the leave

        public ApplicationUser? Approver { get; set; } // Navigation property

        public DateTime? ApprovalDate { get; set; }

        // ✅ Add this missing property
        [Required]
        public DateTime? RequestDate { get; set; } = DateTime.Now;
    }

}
