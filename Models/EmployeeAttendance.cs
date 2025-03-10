using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IT15_TripoleMedelTijol.Models
{
    public class EmployeeAttendance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public string EmployeeID { get; set; } = string.Empty;

        public Employee? Employee { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan? ClockIn { get; set; }
        public TimeSpan? ClockOut { get; set; }

        [Required]
        [RegularExpression("^(Present|Absent|Late)$", ErrorMessage = "Invalid status. Only 'Present', 'Absent', or 'Late' are allowed.")]
        public string Status { get; set; } = string.Empty;
    }

}
