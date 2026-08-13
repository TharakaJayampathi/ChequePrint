using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers
{
    public class ChequePrintController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}