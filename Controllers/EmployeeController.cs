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

        public IActionResult ManageEmployees()
        {
            var employees = _context.Employees.ToList(); // Fetch employees
            return View(employees); // Pass employees to the view
        }
    }
}
