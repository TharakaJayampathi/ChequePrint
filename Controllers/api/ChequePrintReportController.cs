using ChequePrint.Interfaces.ChequePrintReport;
using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequePrintReportController : ControllerBase
    {
        private readonly IChequePrintReportRepository _chequePrintReportRepository;

        public ChequePrintReportController(
            IChequePrintReportRepository chequePrintReportRepository)
        {
            _chequePrintReportRepository = chequePrintReportRepository;
        }

        [HttpGet]
        [Route("get-all")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var _res = await _chequePrintReportRepository.GetAllAsync();
                return Ok(_res);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}