using Messaging.Kafka.Consumer;
using Messaging.Kafka.Consumer.Pump;
using Microsoft.Extensions.Hosting;

namespace Messaging.Hosting.Kafka;

public sealed class KafkaConsumerHostedService(IKafkaMessagePump pump) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => pump.RunAsync(stoppingToken);
}