using Messaging.Abstractions;
using Microsoft.Extensions.Hosting;

namespace Messaging.Hosting.Kafka;

public sealed class KafkaConsumerHostedService(IKafkaMessagePump pump) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => pump.RunAsync(stoppingToken);
}