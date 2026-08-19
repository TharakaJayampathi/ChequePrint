using ChequePrint.DTOs.ChequePrint;
using ChequePrint.Interfaces.ChequePrint;
using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequePrintReportController : ControllerBase
    {
        private readonly IChequePrintRepository _chequePrintRepository;

        public ChequePrintReportController(
            IChequePrintRepository chequePrintRepository)
        {
            _chequePrintRepository = chequePrintRepository;
        }

        [HttpPost("cheque-print")]
        public async Task<IActionResult> ChequePrintAsync(CheckPrintDTO model)
        {
            try
            {
                var (content, fileName) = await _chequePrintRepository.ChequePrintAsync(model);
                return File(content, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("cheque-print-attachment-upload")]
        public async Task<IActionResult> ChequePrintAttachmentUploadAsync([FromForm] ChequePrintAttachmentUploadDTO model)
        {
            try
            {
                var (content, fileName) = await _chequePrintRepository.ChequePrintAttachmentUploadAsync(model);
                return File(content, "application/zip", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}