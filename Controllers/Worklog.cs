using Microsoft.AspNetCore.Mvc;

namespace IT15_TripoleMedelTijol.Controllers
{
    public class Worklog : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
