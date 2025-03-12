using IT15_TripoleMedelTijol.Data;
using IT15_TripoleMedelTijol.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IT15_TripoleMedelTijol.Controllers
{
    [Authorize(Roles = "HR,Admin")]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendanceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ClockIn()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "User not found." });

            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone).Date;


            var existingAttendance = await _context.Attendance
                .FirstOrDefaultAsync(a => a.EmployeeId == user.Id && a.Date == today);

            if (existingAttendance != null)
            {
                return Json(new { success = false, message = "You have already clocked in today." });
            }

            var attendance = new Attendance
            {
                EmployeeId = user.Id,
                Date = today,
                ShiftStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone)
            };

            _context.Attendance.Add(attendance);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                shiftStart = attendance.ShiftStart.HasValue
                    ? attendance.ShiftStart.Value.ToString("hh:mm tt")
                    : "Not yet clocked in"
            });
        }

        [HttpPost]
        public async Task<IActionResult> ClockOut()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "User not found." });

            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone).Date;


            var attendance = await _context.Attendance
                .FirstOrDefaultAsync(a => a.EmployeeId == user.Id && a.Date == today);

            if (attendance == null)
            {
                return Json(new { success = false, message = "You need to clock in first." });
            }

            if (attendance.ShiftEnd != null)
            {
                return Json(new { success = false, message = "You have already clocked out today." });
            }

            attendance.ShiftEnd = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);

            await _context.SaveChangesAsync();

            return Json(new { success = true, shiftEnd = attendance.ShiftEnd?.ToString("hh:mm tt") });
        }
    }
}
