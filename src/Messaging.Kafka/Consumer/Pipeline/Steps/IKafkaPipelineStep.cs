namespace Messaging.Kafka.Consumer.Pipeline.Steps;

public interface IKafkaPipelineStep
{
    Task<KafkaConsumeContext> ExecuteAsync(
        KafkaConsumeContext context,
        CancellationToken ct);
}