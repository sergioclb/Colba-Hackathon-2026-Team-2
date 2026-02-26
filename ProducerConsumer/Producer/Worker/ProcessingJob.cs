using Producer.Models;
using Raven.Client.Documents;

namespace Producer.Worker;

public class ProcessingJob(IDocumentStore store, IDocumentWorkerFactory workerFactory, ILogger<ProcessingJob> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en el job");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessPendingMessages(CancellationToken stoppingToken)
    {
        using var session = store.OpenAsyncSession();

        var pendingMessages = await session.Query<ReceivedMessage>()
            .Take(10)
            .ToListAsync(stoppingToken);

        if (pendingMessages.Count == 0)
        {
            logger.LogDebug("No hay mensajes pendientes");
            return;
        }

        logger.LogInformation("Procesando {Count} mensajes", pendingMessages.Count);

        await Parallel.ForEachAsync(pendingMessages, new ParallelOptions
        {
            MaxDegreeOfParallelism = 4,
            CancellationToken = stoppingToken
        }, async (message, ct) =>
        {
            var worker = workerFactory.Create();
            await worker.ProcessAsync(message, ct);
        });
    }
}