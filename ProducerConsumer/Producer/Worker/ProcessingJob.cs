using Producer.Models;
using Raven.Client.Documents;

namespace Producer.Worker;

public class ProcessingJob(IDocumentStore store, IDocumentWorkerFactory workerFactory, ILogger<ProcessingJob> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("✅ ProcessingJob iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessages(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error en el job");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        logger.LogInformation("⏹️ ProcessingJob detenido");
    }

    private async Task ProcessPendingMessages(CancellationToken stoppingToken)
    {
        using var session = store.OpenAsyncSession();

        var pendientes = await session.Query<ReceivedMessage>()
            .Take(10)
            .ToListAsync(stoppingToken);

        logger.LogInformation("🔍 Ciclo de procesamiento: {Count} mensajes encontrados", pendientes.Count);

        if (pendientes.Count == 0)
        {
            return;
        }

        await Parallel.ForEachAsync(pendientes, new ParallelOptions
        {
            MaxDegreeOfParallelism = 4,
            CancellationToken = stoppingToken
        }, async (message, ct) =>
        {
            var worker = workerFactory.Create();
            await worker.ProcessAsync(message.Id, ct);
        });
    }
}