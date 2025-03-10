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
        public string? EmployeeID { get; set; } // Foreign key to Employee

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MonthlySalary { get; set; } // Base salary (monthly)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TwoWeekPay { get; set; } // MonthlySalary / 2

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DailyRate { get; set; } // TwoWeekPay / 12

        [Column(TypeName = "decimal(18, 2)")]
        public decimal HourlyRate { get; set; } // DailyRate / HoursPerDay

        public DateTime EffectiveDate { get; set; } = DateTime.Now; // When the salary was set

        public bool IsCurrent { get; set; } = true; // Marks the latest salary

        // Navigation property
        public Employee? Employee { get; set; }
    }

}