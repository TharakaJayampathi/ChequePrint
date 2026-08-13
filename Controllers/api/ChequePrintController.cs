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

        [HttpPost("ChequePrintAttachmentUpload")]
        public async Task<IActionResult> ChequePrintAttachmentUploadAsync([FromForm] ChequePrintAttachmentUploadDTO model)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}