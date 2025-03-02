using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace IT15_TripoleMedelTijol.Models
{
    public class EmergencyContact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Relationship { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
