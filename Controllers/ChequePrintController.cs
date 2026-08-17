using ChequePrint.Interfaces.ChequePrint;
using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers
{
    public class ChequePrintController : Controller
    {
        private readonly IChequePrintRepository _chequePrintRepository;

        public ChequePrintController(
            IChequePrintRepository chequePrintRepository)
        {
            _chequePrintRepository = chequePrintRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Form()
        {
            return View();
        }

        public IActionResult BulkUpload()
        {
            return View();
        }
    }
}