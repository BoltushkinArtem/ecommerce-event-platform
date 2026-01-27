using Messaging.Hosting.Kafka;
using Messaging.Kafka.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Consumer.Handlers;
using OrderService.Contracts.Events;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting host");
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });
            
            services.AddKafkaCoreMessaging(configuration);
            services.AddKafkaConsumerMessaging(context.Configuration);
            
            services.AddHostedService<KafkaConsumerHostedService>();
            services.AddKafkaHandler<OrderCreated, OrderCreatedHandler>();
        })
        .UseConsoleLifetime()
        .Build();

    Log.Information("Host built successfully");

    await host.RunAsync();

    Log.Information("Host stopped gracefully");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}