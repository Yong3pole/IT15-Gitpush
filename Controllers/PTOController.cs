using IT15_TripoleMedelTijol.Data;
using IT15_TripoleMedelTijol.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace IT15_TripoleMedelTijol.Controllers
{
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

            try
            {
                // Populate navigation properties
                leaveRequest.Employee = await _context.Employees.FindAsync(leaveRequest.EmployeeID);
                leaveRequest.LeaveType = await _context.LeaveTypes.FindAsync(leaveRequest.LeaveTypeId);

                leaveRequest.Status = "Pending"; // Default status
                leaveRequest.RequestDate = DateTime.Now;

                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log database errors
                Console.WriteLine(ex.Message);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
