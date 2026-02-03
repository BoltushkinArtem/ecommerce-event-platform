using Confluent.Kafka;

namespace Messaging.Kafka.Consumer.Pipeline;

public interface IKafkaPipeline
{
    Task<PipelineExecutionResult> ExecuteAsync(
        ConsumeResult<string, string> result,
        CancellationToken ct);
}