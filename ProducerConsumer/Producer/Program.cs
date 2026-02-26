using Producer.Repository;
using Producer.Service;
using Producer.Worker;
using Raven.Client.Documents;
using Raven.Client.Exceptions;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageServiceImp>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDocumentStore>(_ =>
{
    var storeUrl = Environment.GetEnvironmentVariable("RAVEN_URL") ?? "http://localhost:8080";
    var databaseName = Environment.GetEnvironmentVariable("RAVEN_DATABASE") ?? "Hackathon";
    
    var store = new DocumentStore
    {
        Urls = [storeUrl],
        Database = databaseName
    };
    
    store.Initialize();
    // Crea la base de datos si no existe
    try
    {
        store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(databaseName)));
    }
    catch (ConcurrencyException)
    {
        // La base de datos ya existe, ignoramos
    }
    
    return store;
});

builder.Services.AddHttpClient();
builder.Services.AddTransient<IDocumentWorker, DocumentWorker>();
builder.Services.AddSingleton<IDocumentWorkerFactory, DocumentWorkerFactory>();
builder.Services.AddHostedService<ProcessingJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();