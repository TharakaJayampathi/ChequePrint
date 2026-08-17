using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Logoff()
        {
            return RedirectToAction("Index", "Auth");
        }
    }
}