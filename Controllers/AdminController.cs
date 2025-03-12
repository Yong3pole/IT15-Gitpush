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
    // Allow both HR and Admin roles to access this controller
    [Authorize(Roles = "HR,Admin")]
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

            // Validate and save resume file
            if (ResumeFile != null && ResumeFile.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf" };
                var fileExtension = Path.GetExtension(ResumeFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Only PDF resumes are allowed.";
                    return RedirectToAction("Applicants", "Admin");
                }

                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine("wwwroot/uploads/resumes", fileName);
                Directory.CreateDirectory("wwwroot/uploads/resumes");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ResumeFile.CopyToAsync(stream);
                }

                applicant.ResumePath = "/uploads/resumes/" + fileName;
            }

            // Ensure DateApplied is set
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
                    var existingJob = _context.JobPostings
                        .Where(j => j.JobID == jobPosting.JobID)
                        .Select(j => new { j.JobID, j.JobTitleId, j.DepartmentId })
                        .FirstOrDefault();

                    if (existingJob == null)
                    {
                        TempData["ErrorMessage"] = "Job posting not found.";
                        return RedirectToAction("JobPostings");
                    }

                    var jobTitle = _context.JobTitles.Find(existingJob.JobTitleId);
                    var department = _context.Departments.Find(existingJob.DepartmentId);

                    if (jobTitle == null || department == null)
                    {
                        TempData["ErrorMessage"] = "Job Title or Department not found.";
                        return RedirectToAction("JobPostings");
                    }

                    var jobToUpdate = _context.JobPostings.Find(jobPosting.JobID);
                    jobToUpdate.Status = jobPosting.Status;

                    if (jobPosting.Status == "Closed" && jobPosting.HiredApplicantID.HasValue)
                    {
                        jobToUpdate.HiredApplicantID = jobPosting.HiredApplicantID;

                        var hiredApplicant = _context.Applicants.Find(jobPosting.HiredApplicantID);
                        if (hiredApplicant != null)
                        {
                            hiredApplicant.Status = "Hired";

                            var existingEmployee = _context.Employees
                                .FirstOrDefault(e => e.ApplicantID == hiredApplicant.ApplicantID);

                            if (existingEmployee == null)
                            {
                                var newEmployee = new Employee
                                {
                                    ApplicantID = hiredApplicant.ApplicantID,
                                    FirstName = hiredApplicant.FirstName,
                                    LastName = hiredApplicant.LastName,
                                    Gender = hiredApplicant.Gender,
                                    DateOfBirth = hiredApplicant.DateOfBirth,
                                    Email = hiredApplicant.Email,
                                    Phone = hiredApplicant.Phone,
                                    HouseNumber = hiredApplicant.HouseNumber,
                                    Street = hiredApplicant.Street,
                                    Barangay = hiredApplicant.Barangay,
                                    City = hiredApplicant.City,
                                    Province = hiredApplicant.Province,
                                    ZipCode = hiredApplicant.ZipCode,
                                    ResumePath = hiredApplicant.ResumePath,
                                    DateHired = DateTime.Now,
                                    EmploymentStatus = true,

                                    JobTitle = jobTitle,
                                    Department = department
                                };

                                _context.Employees.Add(newEmployee);
                            }

                            _context.SaveChanges();
                            TempData["SuccessMessage"] = $"{hiredApplicant.FullName} has been hired and added to Employees.";
                            return RedirectToAction("Onboarding", "Onboarding");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Selected applicant not found.";
                            return RedirectToAction("JobPostings");
                        }
                    }

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Job posting updated successfully.";
                    return RedirectToAction("JobPostings");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                    return RedirectToAction("JobPostings");
                }
            }

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