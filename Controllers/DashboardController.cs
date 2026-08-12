using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}