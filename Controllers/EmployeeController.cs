using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IT15_TripoleMedelTijol.Models;
using IT15_TripoleMedelTijol.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace IT15_TripoleMedelTijol.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context; // Inject DB Context

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["DepartmentCount"] = _context.Departments.Count();
            ViewData["EmployeeCount"] = _context.Employees.Count();
            return View();
        }

        // For Modal view of employee details
        public async Task<IActionResult> GetEmployeeDetails(string id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Applicant)
                .Include(e => e.Salaries) // Include Salaries
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
            {
                return NotFound();
            }

            return PartialView("_EmployeeDetailsPartial", employee);
        }

        // For Modal Editing View of employee details
        public async Task<IActionResult> GetEmployeeForEdit(string id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.Applicant)
                .Include(e => e.Salaries) // Include Salaries
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
            {
                return NotFound();
            }

            return PartialView("_EmployeeEditPartial", employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateSalary([FromBody] Salary salaryData)
        {
            if (!ModelState.IsValid || salaryData.MonthlySalary <= 0)
            {
                return Json(new { success = false, message = "Invalid data or monthly salary must be greater than 0." });
            }

            // Check if the employee exists
            var employee = await _context.Employees
                .Include(e => e.Salaries)
                .FirstOrDefaultAsync(e => e.EmployeeID == salaryData.EmployeeID);

            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found." });
            }

            // If updating an existing salary, mark the old one as inactive
            if (salaryData.IsCurrent)
            {
                var currentSalary = employee.Salaries.FirstOrDefault(s => s.IsCurrent);
                if (currentSalary != null)
                {
                    currentSalary.IsCurrent = false;
                }
            }

            // Calculate DailyRate and HourlyRate
            decimal twoWeekPay = salaryData.MonthlySalary / 2; // Payroll is every two weeks
            decimal dailyRate = twoWeekPay / 12; // 6 days a week (12 days in two weeks)
            decimal hourlyRate = dailyRate / 8; // Assuming 8 hours per day

            // Add the new salary record
            var newSalary = new Salary
            {
                EmployeeID = salaryData.EmployeeID,
                MonthlySalary = salaryData.MonthlySalary,
                TwoWeekPay = twoWeekPay,
                DailyRate = dailyRate,
                HourlyRate = hourlyRate,
                EffectiveDate = salaryData.EffectiveDate,
                IsCurrent = salaryData.IsCurrent
            };

            _context.Salaries.Add(newSalary);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Salary saved successfully!" });
        }


        // View All Employees Page
        public IActionResult ManageEmployees()
        {
            var FetchAllEmployees = _context.Employees
                .Include(e => e.Department)  // Ensure Department is loaded
                .Include(e => e.JobTitle)    // Ensure JobTitle is loaded
                .ToList();

            return View(FetchAllEmployees);
        }

    }
}
