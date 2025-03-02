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
    [Authorize(Roles = "HR")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // View Employees from the Employees table
        public async Task<IActionResult> EmployeeManagement()
        {
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }

        // View Employee details in UserManagement page
        public async Task<IActionResult> GetEmployeeDetails(string id)
        {
            // Fetch the employee
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Fetch the current salary
            var currentSalary = await _context.Salaries
                .Where(s => s.EmployeeID == id) // Use 'id' instead of 'employeeId'
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();

            // Pass both employee and currentSalary to the view
            ViewBag.CurrentSalary = currentSalary?.Amount.ToString("C") ?? "N/A"; // Format as currency or display "N/A"
            return PartialView("_EmployeeDetailsPartial", employee);
        }

        public IActionResult PayrollAttendance()
        {
            return View();
        }

        public IActionResult PTO()
        {
            return View();
        }


        ///////////////////////////////////////////// APPLICANTS /////////////////////////////////////////////
        // ✅ Display Applicants Page
        public async Task<IActionResult> Applicants()
        {
            var applicants = await _context.Applicants
                .Include(a => a.JobPosting)
                .ThenInclude(j => j.JobTitle) // Ensure JobTitle is loaded
                .ToListAsync();


            ViewBag.JobPostings = await _context.JobPostings.Include(j => j.JobTitle).ToListAsync();
            return View(applicants);
        }

        // ✅ Handle New Applicant Submission
        [HttpPost]
        public async Task<IActionResult> AddApplicant(Applicant applicant, IFormFile ResumeFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to add applicant. Please check the form.";
                return RedirectToAction("Applicants", "Admin");
            }

            if (ResumeFile != null && ResumeFile.Length > 0)
            {
                // Generate a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ResumeFile.FileName);
                var filePath = Path.Combine("wwwroot/uploads/resumes", fileName);

                // Ensure the directory exists
                Directory.CreateDirectory(Path.Combine("wwwroot/uploads/resumes"));

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ResumeFile.CopyToAsync(stream);
                }

                // Store the relative path in the database
                applicant.ResumePath = "/uploads/resumes/" + fileName;
            }

            applicant.DateApplied = DateTime.Now;

            _context.Applicants.Add(applicant);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Applicant added successfully!";
            return RedirectToAction("Applicants", "Admin");
        }


        /////////////////////////////////// JOB POSTINGS //////////////////////////////////

        public async Task<IActionResult> JobPostings()
        {
            // Fetch Job Postings with related data
            var jobPostings = await _context.JobPostings
                .Include(j => j.Applicants)
                .Include(j => j.Department)
                .Include(j => j.JobTitle)
                .ToListAsync();

            // Fetch Departments for the dropdown
            var departments = await _context.Departments.ToListAsync();
            ViewBag.Departments = departments; // Ensure this line is present

            return View(jobPostings);
        }

        // Add Job Posting
        [HttpPost]
        public async Task<IActionResult> AddJob(JobPosting job)
        {
            if (!ModelState.IsValid)
            {
                // Log model state errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }

                TempData["ErrorMessage"] = "Failed to add job posting. Please check the form.";
                var jobPostings = await _context.JobPostings.ToListAsync();
                return RedirectToAction("JobPostings", "Admin");
            }

            // Log the job object being passed to the action
            Console.WriteLine($"JobPosting Data: DepartmentId={job.DepartmentId}, JobTitleId={job.JobTitleId}, Description={job.Description}, SalaryRange={job.SalaryRange}, Status={job.Status}");

            // Set the DatePosted to the current date and time
            job.DatePosted = DateTime.Now;

            // Add the job posting to the database
            _context.JobPostings.Add(job);
            await _context.SaveChangesAsync();

            // Success message
            TempData["SuccessMessage"] = "Job posting added successfully!";
            return RedirectToAction("JobPostings", "Admin");

        }

        [HttpGet]
        public async Task<IActionResult> GetJobTitles(int departmentId)
        {
            var jobTitles = await _context.JobTitles
                .Where(jt => jt.DepartmentId == departmentId)
                .Select(jt => new { jt.JobTitleId, jt.Name })
                .ToListAsync();

            return Json(jobTitles);
        }

        // Edit Job Posting
        [HttpPost]
        public IActionResult EditJob(JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingJob = _context.JobPostings.Find(jobPosting.JobID);
                    if (existingJob == null)
                    {
                        TempData["ErrorMessage"] = "Job posting not found.";
                        return RedirectToAction("JobPostings");
                    }

                    // Update job posting fields
                    existingJob.Status = jobPosting.Status;

                    // If job status is "Closed" and a HiredApplicantID is selected
                    if (jobPosting.Status == "Closed" && jobPosting.HiredApplicantID.HasValue)
                    {
                        existingJob.HiredApplicantID = jobPosting.HiredApplicantID;

                        // Update the hired applicant's status to "Hired"
                        var hiredApplicant = _context.Applicants.Find(jobPosting.HiredApplicantID);
                        if (hiredApplicant != null)
                        {
                            hiredApplicant.Status = "Hired";

                            // Store the hired applicant's details in TempData
                            TempData["HiredApplicantFirstName"] = hiredApplicant.FirstName;
                            TempData["HiredApplicantLastName"] = hiredApplicant.LastName;
                            TempData["HiredApplicantEmail"] = hiredApplicant.Email;
                            TempData["HiredApplicantPhone"] = hiredApplicant.Phone;

                            // Set success message with the hired applicant's name
                            TempData["SuccessMessage"] = $"This job posting is now closed, {hiredApplicant.FullName} has been hired.";

                            // Save changes to the database
                            _context.SaveChanges();

                            // Redirect to the Onboarding view
                            return RedirectToAction("Onboarding", "Onboarding");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Selected applicant not found.";
                            return RedirectToAction("JobPostings");
                        }
                    }
                    else
                    {
                        // Set success message for other status updates
                        TempData["SuccessMessage"] = "Job posting updated successfully.";
                        _context.SaveChanges();
                        return RedirectToAction("JobPostings");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (ex) if needed
                    TempData["ErrorMessage"] = "An error occurred while updating the job posting.";
                    return RedirectToAction("JobPostings");
                }
            }

            // If we got this far, something failed; redisplay the form
            TempData["ErrorMessage"] = "Invalid data submitted.";
            return RedirectToAction("JobPostings");
        }

        public IActionResult GetApplicantsByJobId(int jobId)
        {
            if (jobId <= 0)
            {
                return BadRequest("Invalid Job ID");
            }

            try
            {
                var applicants = _context.Applicants
                    .Where(a => a.JobID == jobId)
                    .Select(a => new { applicantID = a.ApplicantID, name = a.FullName })
                    .ToList();

                return Json(applicants);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if needed
                return StatusCode(500, "An error occurred while fetching applicants.");
            }
        }

        

    }
}