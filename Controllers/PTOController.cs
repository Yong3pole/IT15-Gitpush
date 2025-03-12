using IT15_TripoleMedelTijol.Data;
using IT15_TripoleMedelTijol.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IT15_TripoleMedelTijol.Controllers
{
    [Authorize(Roles = "HR,Admin")]
    public class PTOController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PTOController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(lr => lr.Employee)
                .Include(lr => lr.LeaveType)
                .ToListAsync();

            ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "EmployeeID", "FullName");
            ViewBag.LeaveTypes = new SelectList(await _context.LeaveTypes.ToListAsync(), "LeaveTypeId", "Name");

            return View(leaveRequests);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitLeaveRequest(LeaveRequest leaveRequest)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors
                ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "EmployeeID", "FullName");
                ViewBag.LeaveTypes = new SelectList(await _context.LeaveTypes.ToListAsync(), "LeaveTypeId", "Name");
                return View("Index", await _context.LeaveRequests
                    .Include(lr => lr.Employee)
                    .Include(lr => lr.LeaveType)
                    .ToListAsync());
            }

            // Validate EmployeeID and LeaveTypeId
            var employee = await _context.Employees.FindAsync(leaveRequest.EmployeeID);
            var leaveType = await _context.LeaveTypes.FindAsync(leaveRequest.LeaveTypeId);

            if (employee == null || leaveType == null)
            {
                ModelState.AddModelError("", "Invalid Employee or Leave Type selected.");
                ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "EmployeeID", "FullName");
                ViewBag.LeaveTypes = new SelectList(await _context.LeaveTypes.ToListAsync(), "LeaveTypeId", "Name");
                return View("Index", await _context.LeaveRequests
                    .Include(lr => lr.Employee)
                    .Include(lr => lr.LeaveType)
                    .ToListAsync());
            }

            try
            {
                // Populate navigation properties
                leaveRequest.Employee = employee;
                leaveRequest.LeaveType = leaveType;

                leaveRequest.Status = "Pending"; // Default status
                leaveRequest.RequestDate = DateTime.Now;

                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log database errors
                Console.WriteLine(ex.InnerException?.Message); // Log the inner exception for more details
                ModelState.AddModelError("", "An error occurred while saving the leave request. Please try again.");
                ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "EmployeeID", "FullName");
                ViewBag.LeaveTypes = new SelectList(await _context.LeaveTypes.ToListAsync(), "LeaveTypeId", "Name");
                return View("Index", await _context.LeaveRequests
                    .Include(lr => lr.Employee)
                    .Include(lr => lr.LeaveType)
                    .ToListAsync());
            }

            return RedirectToAction("Index");
        }
    }
}