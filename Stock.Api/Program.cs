using Infra.Shared.ServiceDiscovery;
using Infra.Shared.StaticLogger;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddStaticLogger();

var serviceConfig = builder.Configuration.GetServiceConfig();

builder.Services.RegisterConsulServices(serviceConfig);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("_healthcheck/status");

app.MapGet("/", () => "Hello World!");

app.Run();
