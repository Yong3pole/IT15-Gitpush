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
