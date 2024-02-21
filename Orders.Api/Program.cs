using Infra.Shared.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

var serviceConfig = builder.Configuration.GetServiceConfig();

builder.Services.RegisterConsulServices(serviceConfig);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("_healthcheck/status");

app.MapGet("/hello", () => "Hello World - orders!");

app.Run();
