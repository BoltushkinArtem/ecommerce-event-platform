namespace Messaging.Kafka.Consumer;

public interface IKafkaMessagePump
{
    Task RunAsync(CancellationToken ct);
}