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

        [HttpPost]
        [Route("cheque-print")]
        public async Task<IActionResult> ChequePrintAsync(ChequePrintDTO model)
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

        [HttpPost]
        [Route("cheque-print-attachment-upload")]
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