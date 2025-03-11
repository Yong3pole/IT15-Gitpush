namespace IT15_TripoleMedelTijol.Models
{
    public class PayrollViewModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PresentDays { get; set; }
        public int LateMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
        public decimal GrossEarnings { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal PhilHealthContribution { get; set; }
        public decimal SSSContribution { get; set; }
        public decimal PagIBIGContribution { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetEarnings { get; set; }
    }
}
