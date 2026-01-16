using Blockpit.Api.Extensions;
using Blockpit.Api.Helpers;
using Blockpit.Configuration;
using Blockpit.Data;
using Blockpit.Listener;
using Blockpit.Mediator.Handlers;
using Blockpit.Migrations;
using Blockpit.Observability;
using Blockpit.Query;
using FluentMigrator.Runner;
using LinqToDB;
using log4net;
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

var settings = new Settings();
builder.Services.AddSingleton(settings);

builder.Logging.ClearProviders();
builder.Services.AddSingleton<ILog>(_ => ConfigureLog4Net.FromSettings(settings));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<DatabaseStoreBlockHandler>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Allow", policy =>
    {
        policy.WithOrigins(settings.CorsOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHostedService<Listener>();

var centralCounterService = new CentralCounterService();
builder.Services.AddSingleton(centralCounterService);

builder.Services.AddHostedService(sp => sp.GetRequiredService<CentralCounterService>());

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSQLite()
        .WithGlobalConnectionString(settings.ConnectionString)
        .ScanIn(typeof(Migration20260113125200).Assembly).For.All());

var app = builder.Build();

app.UseFluentMigrator();

app.MapGet("/mocks/btc", () =>
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Mocks/Btc.json");
        return Results.File(filePath, "application/json");
    })
    .WithName("BTC");

app.MapGet("/mocks/eth", () =>
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Mocks/Eth.json");
        return Results.File(filePath, "application/json");
    })
    .WithName("ETH");

app.MapGet("/mocks/dash", () =>
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Mocks/Dash.json");
        return Results.File(filePath, "application/json");
    })
    .WithName("DASH");

app.MapGet("/health", () =>
        centralCounterService.GetEventCounts())
    .WithName("Health");

app.MapGet("/fetch/{symbol}", (string symbol) =>
    {
        var lastWeek = DateTime.Now.AddDays(settings.FetchDateOffsetDays * -1);

        var options = new DataOptions().UseSQLite(settings.ConnectionString);
        var query = new DenormalizedBlockTicksQuery(new DbContext(options));
        var values = query.ExecuteAsync(symbol, lastWeek, settings.FetchLimit);

        centralCounterService.AddEvent("FetchAPI");
        return values;
    })
    .WithName("Fetch");

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
