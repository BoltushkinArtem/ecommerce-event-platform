using Messaging.Hosting.Kafka;
using Messaging.Kafka.DependencyInjection;
using OrderService.Contracts.Events;
using OrderService.Worker.Handlers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services);
});

builder.Services.AddKafkaCoreMessaging(builder.Configuration);
builder.Services.AddKafkaConsumerMessaging(builder.Configuration);

builder.Services.AddKafkaHandler<OrderCreated, OrderCreatedHandler>();
builder.Services.AddHostedService<KafkaConsumerHostedService>();

var host = builder.Build();

try
{
    Log.Information("Starting OrderService.Worker");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "OrderService.Worker terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}