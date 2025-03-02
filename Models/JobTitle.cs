using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    public class JobTitle
    {
        [Key]
        public int JobTitleId { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Add DepartmentId foreign key
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        // Navigation property to Department
        public Department Department { get; set; }
    }
}