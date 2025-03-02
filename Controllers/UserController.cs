using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using IT15_TripoleMedelTijol.Models; // Ensure this points to your ApplicationUser model

namespace IT15_TripoleMedelTijol.Controllers
{
    [Authorize] // Ensure only logged-in users can access
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not found
            }

            return View(user); // Pass user object to the view
        }

        public IActionResult Tasks()
        {
            return View();
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult Payments()
        {
            return View();
        }
    }
}
