using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


using System.ComponentModel.DataAnnotations;

namespace IT15_TripoleMedelTijol.Models
{
    public class LeaveType
    {
        [Key]
        public int LeaveTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty; // e.g., "Sick Leave", "Vacation Leave"

        [Required]
        public int DefaultDays { get; set; } // Default allocation of days per year

        public bool IsPaid { get; set; } // Whether this leave type is paid or unpaid
    }

}
