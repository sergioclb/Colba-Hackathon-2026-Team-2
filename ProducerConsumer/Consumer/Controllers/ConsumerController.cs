using Microsoft.AspNetCore.Mvc;

namespace Consumer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerController : ControllerBase
    {
        [HttpPost]
        public IActionResult ReceiveMessage([FromBody] MessageRequest request)
        {
            if (request == null)
                return BadRequest("Payload cannot be null.");

            if (string.IsNullOrWhiteSpace(request.Payload))
                return BadRequest("Payload cannot be empty.");

            if (!IsValidFormat(request.Payload))
                return BadRequest("Invalid payload format.");

            return Ok(new
            {
                Status = "Message received successfully"
            });
        }

        private bool IsValidFormat(string payload)
        {
            return payload.Length >= 3;
        }
    }

    public class MessageRequest
    {
        public string Payload { get; set; }
    }
}