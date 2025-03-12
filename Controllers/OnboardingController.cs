using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT15_TripoleMedelTijol.Controllers
{
    [Authorize(Roles = "HR,Admin")]
    public class OnboardingController : Controller
    {
        public IActionResult OnboardingLandingPage()
        {
            // You can pass any necessary data to the view here
            return View();
        }

        public IActionResult Onboarding()
        {
            // Pass TempData values to ViewBag
            ViewBag.HiredApplicantFirstName = TempData["HiredApplicantFirstName"]?.ToString();
            ViewBag.HiredApplicantLastName = TempData["HiredApplicantLastName"]?.ToString();
            ViewBag.HiredApplicantEmail = TempData["HiredApplicantEmail"]?.ToString();
            ViewBag.HiredApplicantPhone = TempData["HiredApplicantPhone"]?.ToString();

            // Ensure the success message is preserved for SweetAlert
            ViewBag.SuccessMessage = TempData["SuccessMessage"]?.ToString();

            return View();
        }
    }
}
