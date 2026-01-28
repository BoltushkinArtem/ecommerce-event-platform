using Confluent.Kafka;
using Messaging.Kafka.Consumer.Dispatching;
using Messaging.Kafka.Consumer.Factories;
using Messaging.Kafka.Consumer.Registry;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pump;

public sealed class KafkaMessagePump(
    IKafkaConsumerFactory consumerFactory,
    IKafkaHandlerRegistry registry,
    IKafkaMessageDispatcher dispatcher,
    ILogger<KafkaMessagePump> logger)
    : IKafkaMessagePump
{
    private readonly IConsumer<string, string> _consumer =
        consumerFactory.Create();

    public async Task RunAsync(CancellationToken ct)
    {
        _consumer.Subscribe(registry.Topics);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var result = _consumer.Consume(ct);
                if (result is null) continue;

                await dispatcher.DispatchAsync(result, ct);
                _consumer.Commit(result);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            logger.LogInformation("Kafka consumer shutdown requested");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Kafka message");
            throw;
        }
        finally
        {
            try
            {
                _consumer.Close();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error while closing Kafka consumer");
            }
        }
    }
}