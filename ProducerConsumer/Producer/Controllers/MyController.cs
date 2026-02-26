using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class MyController(IDocumentStore store) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        var persona = new Persona
        {
            Nombre = "Fran",
            Edad = 39
        };

        using var session = store.OpenAsyncSession();
        await session.StoreAsync(persona);
        await session.SaveChangesAsync();

        return Ok(persona.Id);
    }
}

public record PersonaRequest(string Nombre, int Edad);

public class Persona
{
    public string Id { get; set; }   // RavenDB genera el Id automáticamente (ej: "personas/1-A")
    public string Nombre { get; set; }
    public int Edad { get; set; }
}