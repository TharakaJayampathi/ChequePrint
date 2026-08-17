using ChequePrint.DTOs.ChequePrint;
using ChequePrint.Interfaces.ChequePrint;
using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequePrintController : ControllerBase
    {
        private readonly IChequePrintRepository _chequePrintRepository;

        public ChequePrintController(
            IChequePrintRepository chequePrintRepository)
        {
            _chequePrintRepository = chequePrintRepository;
        }

        [HttpPost("cheque-print")]
        public async Task<IActionResult> ChequePrintAsync(CheckPrintDTO model)
        {
            try
            {
                await _chequePrintRepository.ChequePrintAsync(model);
                return Ok();
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