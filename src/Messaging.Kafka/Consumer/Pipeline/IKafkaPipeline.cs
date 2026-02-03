using Confluent.Kafka;

namespace Messaging.Kafka.Consumer.Pipeline;

public interface IKafkaPipeline
{
    Task<KafkaProcessingResult> ExecuteAsync(
        ConsumeResult<string, string> result,
        CancellationToken ct);
}