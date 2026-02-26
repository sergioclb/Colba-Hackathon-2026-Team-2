using Producer.Extensions;
using Producer.Models;
using Raven.Client.Documents;

namespace Producer.Worker;

public interface IDocumentWorker
{
    Task ProcessAsync(ReceivedMessage message, CancellationToken ct);
}

public class DocumentWorker(IDocumentStore store, IHttpClientFactory httpClientFactory, ILogger<DocumentWorker> logger)
    : IDocumentWorker
{
    public async Task ProcessAsync(ReceivedMessage message, CancellationToken ct)
    {
        using var session = store.OpenAsyncSession();

        try
        {
            // 1. Convert from ReceivedMessageToProcessingMessage and store
            var processingMessage = message.ToProcessingMessage();
            await session.StoreAsync(processingMessage, ct);
            
            // 2. Delete ReceivedMessage
            session.Delete(message);
            await session.SaveChangesAsync(ct);

            // 3. Send Payload
            var client = httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(
                message.DestinationUrl, 
                message.Payload, 
                cancellationToken: ct
            );
            response.EnsureSuccessStatusCode();

            // 4. Store it as processed
            var processedMessage = new ProcessedMessage
            {
                Id = message.Id,
                Payload = message.Payload,
                DestinationUrl = message.DestinationUrl
            };
            
            await session.StoreAsync(processedMessage, ct);
            session.Delete(processingMessage.Id);
            await session.SaveChangesAsync(ct);

            logger.LogInformation("Message with {Id} correctly processed", message.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message {Id}", message.Id);
            
            // Store as error
            var errorMessage = new ErrorMessage
            {
                Id = message.Id,
                Payload = message.Payload,
                DestinationUrl = message.DestinationUrl
            };
            
            await session.StoreAsync(errorMessage, ct);
            session.Delete(message);
            await session.SaveChangesAsync(ct);
        }
    }
}