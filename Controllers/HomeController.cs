using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using IT15_TripoleMedelTijol.Models;
using IT15_TripoleMedelTijol.Data;
using Microsoft.EntityFrameworkCore;

namespace IT15_TripoleMedelTijol.Controllers
{
    [Authorize(Roles = "HR,Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserDashboard()
        {
            return View();
        }

        [Authorize(Roles = "HR,Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            return View();
        }
    }
}
