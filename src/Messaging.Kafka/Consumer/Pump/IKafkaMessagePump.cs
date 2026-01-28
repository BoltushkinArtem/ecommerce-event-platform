namespace Messaging.Kafka.Consumer.Pump;

public interface IKafkaMessagePump
{
    Task RunAsync(CancellationToken ct);
}