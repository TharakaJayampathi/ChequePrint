using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers
{
    public class CheckPrintReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}