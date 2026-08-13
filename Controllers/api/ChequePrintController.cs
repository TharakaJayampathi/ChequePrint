using ChequePrint.DTOs.ChequePrint;
using Microsoft.AspNetCore.Mvc;

namespace ChequePrint.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequePrintController : ControllerBase
    {
        public ChequePrintController()
        { 
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