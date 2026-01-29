using Messaging.Kafka.DependencyInjection;
using OrderService.Service.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
    cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddOpenApi();

builder.Services.AddKafkaCoreMessaging(builder.Configuration);
builder.Services.AddKafkaProducerMessaging(builder.Configuration);

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Log.Information(
        "OrderService.Service started successfully. Environment={Environment}, Urls={Urls}",
        app.Environment.EnvironmentName,
        string.Join(", ", app.Urls));
});

app.Lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("OrderService.Service is stopping...");
});

app.Lifetime.ApplicationStopped.Register(() =>
{
    Log.Information("OrderService.Service has stopped");
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapCreateOrder();

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "OrderService.Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
