using Producer.Worker;
using Raven.Client.Documents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IDocumentStore>(_ =>
{
    var store = new DocumentStore
    {
        Urls = ["http://localhost:8080"],
        Database = "Hackaton"
    };
    store.Initialize();
    return store;
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IDocumentWorker, DocumentWorker>();
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