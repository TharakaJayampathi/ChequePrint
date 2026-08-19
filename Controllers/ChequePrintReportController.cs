using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers
{
    public class ChequePrintReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}