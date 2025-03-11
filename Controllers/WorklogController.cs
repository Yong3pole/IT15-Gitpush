using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IT15_TripoleMedelTijol.Models;
using IT15_TripoleMedelTijol.Data; // Adjust namespace accordingly

namespace IT15_TripoleMedelTijol.Controllers
{
    public class WorklogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorklogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var existingDates = await _context.EmployeeAttendances
                .Select(a => a.Date.Date)
                .Distinct()
                .ToListAsync();

            ViewBag.ExistingAttendanceDates = existingDates.Select(d => d.ToString("yyyy-MM-dd")).ToList();

            return View();
        }

        // Import Attendance Records


        [HttpPost]
        public async Task<IActionResult> ImportAttendance(IFormFile file, DateTime selectedDate)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please upload a valid CSV file.";
                return RedirectToAction("Index");
            }

            var uploadedById = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown"; // Identity Id uploader
            var attendances = new List<EmployeeAttendance>();

            using (var stream = new StreamReader(file.OpenReadStream()))
            {
                await stream.ReadLineAsync(); // Skip header row
                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    var values = line.Split(',');

                    if (values.Length != 5)
                        continue; // Skip invalid rows

                    var employeeId = values[0];
                    if (!await _context.Employees.AnyAsync(e => e.EmployeeID == employeeId))
                        continue; // Skip if EmployeeID does not exist

                    if (!DateTime.TryParse(values[1], out DateTime date))
                        continue; // Skip invalid date

                    // **NEW CHECK: Ensure the date matches the selectedDate**
                    if (date.Date != selectedDate.Date)
                    {
                        TempData["Error"] = $"Attendance date mismatch! Expected: {selectedDate:yyyy-MM-dd}, Found: {date:yyyy-MM-dd}";
                        return RedirectToAction("Index");
                    }

                    TimeSpan? clockIn = TimeSpan.TryParse(values[2], out var inTime) ? inTime : (TimeSpan?)null;
                    TimeSpan? clockOut = TimeSpan.TryParse(values[3], out var outTime) ? outTime : (TimeSpan?)null;
                    var status = values[4];

                    if (!new[] { "Present", "Absent", "Late" }.Contains(status))
                        continue; // Skip invalid status

                    attendances.Add(new EmployeeAttendance
                    {
                        EmployeeID = employeeId,
                        Date = date,
                        ClockIn = clockIn,
                        ClockOut = clockOut,
                        Status = status,
                        UploadedBy = uploadedById, // Capture user
                        UploadedAt = DateTime.Now // Capture timestamp
                    });
                }
            }

            if (attendances.Count > 0)
            {
                await _context.EmployeeAttendances.AddRangeAsync(attendances);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance records imported successfully!";
            }
            else
            {
                TempData["Error"] = "No valid records found.";
            }

            return RedirectToAction("Index");
        }

        // View Attendance Records

        public IActionResult AttendanceCalendar()
        {
            return View(); // Ensure this maps to Views/Worklog/ViewAttendance.cshtml
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceSummary()
        {
            var summary = await _context.EmployeeAttendances
                .GroupBy(a => a.Date)
                .Select(g => new
                {
                    title = $"✅ {g.Count(a => a.Status == "Present")} | ❌ {g.Count(a => a.Status == "Absent")} | ⏳ {g.Count(a => a.Status == "Late")}",
                    start = g.Key.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return Json(summary);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceDetails(DateTime date)
        {
            var records = await _context.EmployeeAttendances
                .Where(a => a.Date == date)
                .Include(a => a.Employee)
                .ToListAsync();

            if (!records.Any())
            {
                return Content("<p>No records found for this date.</p>", "text/html");
            }

            var html = "<ul class='list-group'>";
            foreach (var record in records)
            {
                html += $"<li class='list-group-item'>{record.Employee.FullName} - {record.Status} ({record.ClockIn} - {record.ClockOut})</li>";
            }
            html += "</ul>";

            return Content(html, "text/html");
        }



    }
}


