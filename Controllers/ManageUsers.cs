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
    public class ManageUsers : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
