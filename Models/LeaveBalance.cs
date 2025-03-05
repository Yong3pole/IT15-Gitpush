using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IT15_TripoleMedelTijol.Models
{
    public class LeaveBalance
    {
        [Key]
        public int LeaveBalanceId { get; set; }

        [ForeignKey("Employee")]
        public required string EmployeeID { get; set; } // FK to Employee

        public required Employee Employee { get; set; } // Navigation property

        [ForeignKey("LeaveType")]
        public int LeaveTypeId { get; set; } // FK to LeaveType

        public required LeaveType LeaveType { get; set; } // Navigation property

        public int AvailableDays { get; set; } // Remaining leave balance
    }


}
