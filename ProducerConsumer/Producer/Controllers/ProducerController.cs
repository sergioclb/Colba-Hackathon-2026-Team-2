using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Producer.Models;

namespace Producer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProducerController : ControllerBase
{
    [HttpPost]
    public IActionResult GetMessage(string payload, string url)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(new
        {
            mensaje = "Datos recibidos correctamente",
            payloadRecibido = payload,
            urlRecibida = url
        });
    }
}