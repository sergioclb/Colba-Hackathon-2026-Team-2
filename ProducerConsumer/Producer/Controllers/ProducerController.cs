using Microsoft.AspNetCore.Mvc;
using Producer.Models;
using Raven.Client.Documents;

namespace Producer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProducerController(IDocumentStore store) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> GetMessage(
        [FromForm] string payload,
        [FromForm] string destinationUrl)
    {
        if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(destinationUrl))
            return BadRequest("Faltan parámetros");

        var message = new ReceivedMessage
        {
            Id = Guid.NewGuid().ToString(),
            Payload = payload,
            DestinationUrl = destinationUrl,
            CreatedAt = DateTime.UtcNow
        };
        
        var session = store.OpenAsyncSession();

        await session.StoreAsync(message);
        await session.SaveChangesAsync();

        return Ok();
    }
}