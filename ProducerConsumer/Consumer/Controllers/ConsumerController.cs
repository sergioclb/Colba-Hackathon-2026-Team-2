using Microsoft.AspNetCore.Mvc;

namespace Consumer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumerController : ControllerBase
    {
        [HttpPost]
        public IActionResult ReceiveMessage([FromBody]string request)
        {
            if (request == null)
                return BadRequest("Payload cannot be null.");

            if (string.IsNullOrWhiteSpace(request))
                return BadRequest("Payload cannot be empty.");

            if (!IsValidFormat(request))
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