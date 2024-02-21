using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();



builder.Configuration
    .AddJsonFile("ocelot.json", optional: true);

builder.Services.AddOcelot(builder.Configuration).AddConsul();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/_healthcheck/status");

app.MapWhen(ctx => !ctx.Request.Path.StartsWithSegments("/_health"), app => app.UseOcelot().Wait());

app.Run();