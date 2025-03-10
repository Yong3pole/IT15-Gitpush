using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IT15_TripoleMedelTijol.Models
{
    public class Salary
    {
        [Key]
        public int SalaryID { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeID { get; set; } // Foreign key to Employee

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime EffectiveDate { get; set; } = DateTime.Now; // When the salary was set

        public bool IsCurrent { get; set; } = true; // Marks the latest salary

        // Navigation property
        public Employee? Employee { get; set; }
    }

}