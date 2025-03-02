using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        public string EmployeeId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        public DateTime? ShiftStart { get; set; }
        public DateTime? ShiftEnd { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual ApplicationUser Employee { get; set; }
    }
}
