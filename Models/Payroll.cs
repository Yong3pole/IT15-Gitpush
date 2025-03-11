using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace IT15_TripoleMedelTijol.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollID { get; set; }

        [ForeignKey("Employee")]
        public string EmployeeID { get; set; } // FK to Employee

        public DateTime PeriodStart { get; set; } // Start of payroll period
        public DateTime PeriodEnd { get; set; } // End of payroll period

        [Column(TypeName = "decimal(18, 2)")]
        public decimal GrossPay { get; set; } // Before deductions

        [Column(TypeName = "decimal(18, 2)")]
        public decimal LateDeduction { get; set; } // Deduction for tardiness

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OvertimePay { get; set; } // Overtime earnings

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TaxDeduction { get; set; } // Tax deduction

        [Column(TypeName = "decimal(18, 2)")]
        public decimal SSSDeduction { get; set; } // SSS contribution

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PhilHealthDeduction { get; set; } // PhilHealth contribution

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PagIbigDeduction { get; set; } // Pag-IBIG contribution

        [Column(TypeName = "decimal(18, 2)")]
        public decimal NetPay { get; set; } // Final salary after deductions

        public DateTime ProcessedDate { get; set; } = DateTime.Now; // When payroll was generated

        // Navigation property
        public Employee? Employee { get; set; }
    }

}
