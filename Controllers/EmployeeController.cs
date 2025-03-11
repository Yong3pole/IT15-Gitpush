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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IT15_TripoleMedelTijol.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context; // Inject DB Context

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() // Index //////////////////////////////////
        {
            ViewData["DepartmentCount"] = _context.Departments.Count();
            ViewData["EmployeeCount"] = _context.Employees.Count();
            return View();
        }

        // For Modal view of employee details ///////////////////////////////////
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

        // View All Employees Page /////////////////////////
        public IActionResult ManageEmployees()
        {
            var FetchAllEmployees = _context.Employees
                .Include(e => e.Department)  // Ensure Department is loaded
                .Include(e => e.JobTitle)    // Ensure JobTitle is loaded
                .ToList();

            return View(FetchAllEmployees);
        }

        [HttpPost]
        public IActionResult EditEmployee(Employee employee, decimal monthlySalary, DateTime effectiveDate, bool isCurrent)
        {
            if (ModelState.IsValid)
            {
                // Update employee details
                _context.Update(employee);

                // Add or update salary
                var currentSalary = employee.Salaries.FirstOrDefault(s => s.IsCurrent);
                if (currentSalary != null)
                {
                    currentSalary.IsCurrent = false; // Mark the old salary as not current
                }

                var newSalary = new Salary
                {
                    EmployeeID = employee.EmployeeID,
                    MonthlySalary = monthlySalary,
                    EffectiveDate = effectiveDate,
                    IsCurrent = isCurrent
                };

                _context.Salaries.Add(newSalary);
                _context.SaveChanges();

                return RedirectToAction("ManageEmployees");
            }

            // Repopulate ViewBag for dropdowns if the model is invalid
            ViewBag.JobTitles = _context.JobTitles
                .Select(j => new SelectListItem { Value = j.JobTitleId.ToString(), Text = j.Name })
                .ToList();

            ViewBag.Departments = _context.Departments
                .Select(d => new SelectListItem { Value = d.DepartmentId.ToString(), Text = d.Name })
                .ToList();

            return View(employee);
        }

    }
}
