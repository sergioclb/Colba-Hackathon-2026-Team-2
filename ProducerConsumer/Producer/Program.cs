using Producer.Repository;
using Producer.Service;
using Producer.Worker;
using Raven.Client.Documents;

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
    var store = new DocumentStore
    {
        Urls = new[] { Environment.GetEnvironmentVariable("RAVEN_URL") ?? "http://localhost:8080" },
        Database = Environment.GetEnvironmentVariable("RAVEN_DATABASE") ?? "Hackaton"
    };
    store.Initialize();
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