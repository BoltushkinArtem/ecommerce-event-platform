using Messaging.Abstractions;
using Messaging.Kafka.DependencyInjection;
using Microsoft.Extensions.Configuration;
using OrderService.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting OrderService Producer");

    var services = new ServiceCollection();

    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddSerilog();
    });
    services.AddKafkaMessaging(configuration, KafkaMessagingMode.OnlyProducer);

    var provider = services.BuildServiceProvider();

    var producer = provider.GetRequiredService<IKafkaProducer>();

    await producer.ProduceAsync(
        Guid.NewGuid().ToString(),
        new OrderCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            199.99m,
            DateTime.UtcNow));

    Log.Information("OrderCreated event sent");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}