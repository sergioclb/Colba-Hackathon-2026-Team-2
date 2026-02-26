using Microsoft.AspNetCore.Mvc;
using Producer.Models;
using Producer.Service;

namespace Producer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProducerController : ControllerBase
{
    private readonly IMessageService _service;

    public ProducerController(IMessageService service)
    {
        _service = service;
    }
    [HttpPost]
    public async Task<IActionResult> PostMessage([FromBody] CreateMessageRequest request)
    {
        {
            if (string.IsNullOrEmpty(request.Payload) || string.IsNullOrEmpty(request.Url))
                return BadRequest("Faltan parámetros");

            var message = await _service.ProcessMessageAsync(request.Payload, request.Url);

            return Ok(new
            {
                mensaje = "Datos recibidos correctamente",
                payloadRecibido = message.Payload,
                urlRecibida = message.DestinationUrl
            });
        }
    }
}