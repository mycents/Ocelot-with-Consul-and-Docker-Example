using Ocelot.DependencyInjection;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Configuration
    //.SetBasePath(Directory.GetCurrentDirectory())
    //.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("Ocelot.json", optional: true)
    //.AddEnvironmentVariables()
    ;
// Adiciona suporte ao Ocelot com Consul
builder.Services.AddOcelot(builder.Configuration).AddConsul();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", () => "Hello World - Gateway!");

app.MapHealthChecks("/_healthcheck/status");

app.Run();
