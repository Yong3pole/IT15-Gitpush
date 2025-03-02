using IT15_TripoleMedelTijol.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            int employeeCount = _context.Employees.Count(); // Get the total number of employees
            return View(employeeCount); // Pass the count to the view
        }

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
