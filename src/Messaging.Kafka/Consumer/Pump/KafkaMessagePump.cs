using Messaging.Kafka.Consumer.Factories;
using Messaging.Kafka.Consumer.Pipeline;
using Messaging.Kafka.Consumer.Registry;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pump;

public sealed class KafkaMessagePump(
    IKafkaConsumerFactory consumerFactory,
    IKafkaHandlerRegistry registry,
    IKafkaPipeline pipeline,
    ILogger<KafkaMessagePump> logger)
    : IKafkaMessagePump
{
    public async Task RunAsync(CancellationToken ct)
    {
        using var consumer = consumerFactory.Create();

        
        consumer.Subscribe(registry.Topics);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var result = consumer.Consume(ct);
                if (result is null) continue;
                
                var outcome = await pipeline.ExecuteAsync(result, ct);

                switch (outcome.Decision)
                {
                    case KafkaCommitDecision.Commit:
                        consumer.Commit(result);
                        logger.LogInformation(
                            "Committed offset. Topic={Topic}, Offset={Offset}",
                            result.Topic, result.Offset.Value);
                        break;

                    case KafkaCommitDecision.Skip:
                        logger.LogWarning(
                            "Skipping commit. Topic={Topic}, Offset={Offset}",
                            result.Topic, result.Offset.Value);
                        break;

                    case KafkaCommitDecision.Retry:
                        logger.LogWarning(
                            "Retry requested. Topic={Topic}, Offset={Offset}",
                            result.Topic, result.Offset.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
                consumer.Close();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error while closing Kafka consumer");
            }
        }
    }
}