namespace Messaging.Kafka.Consumer.Pipeline.Steps;

public interface IKafkaPipelineStep
{
    Task ExecuteAsync(
        KafkaMessageContext context,
        CancellationToken ct);
}