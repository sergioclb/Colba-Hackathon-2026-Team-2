using Producer.Extensions;
using Producer.Models;
using Raven.Client.Documents;

namespace Producer.Worker;

public interface IDocumentWorker
{
    Task ProcessAsync(string messageId, CancellationToken ct);
}

public class DocumentWorker(IDocumentStore store, IHttpClientFactory httpClientFactory, ILogger<DocumentWorker> logger)
    : IDocumentWorker
{
    public async Task ProcessAsync(string messageId, CancellationToken ct)
    {
        using var session = store.OpenAsyncSession();

        try
        {
            // 1. Cargar el mensaje desde esta sesión
            var message = await session.LoadAsync<ReceivedMessage>(messageId, ct);
            
            if (message == null)
            {
                logger.LogWarning("Mensaje {Id} no encontrado", messageId);
                return;
            }

            // 2. Convertir a ProcessingMessage
            var processingMessage = message.ToProcessingMessage();
            await session.StoreAsync(processingMessage, ct);
            
            // 3. Eliminar ReceivedMessage (ahora está en esta sesión)
            session.Delete(message);
            await session.SaveChangesAsync(ct);

            // 4. Enviar a destino
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(
                message.DestinationUrl, 
                message.Payload, 
                cancellationToken: ct
            );
            response.EnsureSuccessStatusCode();

            // 5. Marcar como procesado
            var processedMessage = new ProcessedMessage
            {
                Id = message.Id,
                Payload = message.Payload,
                DestinationUrl = message.DestinationUrl
            };
            
            await session.StoreAsync(processedMessage, ct);
            session.Delete(processingMessage.Id);
            await session.SaveChangesAsync(ct);

            logger.LogInformation("✅ Mensaje {Id} procesado correctamente", message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ Error procesando mensaje {Id}", messageId);
            
            using var errorSession = store.OpenAsyncSession();
            
            var message = await errorSession.LoadAsync<ReceivedMessage>(messageId, ct);
            if (message != null)
            {
                var errorMessage = new ErrorMessage
                {
                    Id = message.Id,
                    Payload = message.Payload,
                    DestinationUrl = message.DestinationUrl
                };
                
                await errorSession.StoreAsync(errorMessage, ct);
                errorSession.Delete(message);
                await errorSession.SaveChangesAsync(ct);
            }
        }
    }
}