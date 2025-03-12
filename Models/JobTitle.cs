using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    public class JobTitle
    {
        [Key]
        public int JobTitleId { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public required Department Department { get; set; }

        // ✅ Indicates if the position is currently occupied // nullable
        public bool? IsFilled { get; set; } = false;

        //public Employee? Employee { get; set; } // Navigation property

        public ICollection<Employee> Employees { get; set; } = new List<Employee>(); // One-to-Many
    }
}

