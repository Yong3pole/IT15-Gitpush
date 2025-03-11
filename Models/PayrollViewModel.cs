namespace IT15_TripoleMedelTijol.Models
{
    public class PayrollViewModel
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PresentDays { get; set; }
        public int LateMinutes { get; set; }  // ✅ Added Late Minutes
        public int OvertimeMinutes { get; set; }  // ✅ Added Overtime Minutes
        public decimal TotalEarnings { get; set; }
    }

}
