using Confluent.Kafka;
using Messaging.Kafka.Dispatching;
using Messaging.Kafka.Factories;
using Messaging.Kafka.Registry;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer;

public sealed class KafkaMessagePump(
    IKafkaConsumerFactory consumerFactory,
    IKafkaHandlerRegistry registry,
    IKafkaMessageDispatcher dispatcher,
    ILogger<KafkaMessagePump> logger)
    : IKafkaMessagePump
{
    private readonly IConsumer<string, string> _consumer = consumerFactory.Create();

    public async Task RunAsync(CancellationToken ct)
    {
        _consumer.Subscribe(registry.Topics);

        while (!ct.IsCancellationRequested)
        {
            var result = _consumer.Consume(ct);
            if (result is null) continue;

            try
            {
                await dispatcher.DispatchAsync(result, ct);
                _consumer.Commit(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process message");
                
                throw;
            }
        }
    }
}