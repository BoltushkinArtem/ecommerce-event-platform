namespace Messaging.Abstractions;

public interface IKafkaMessagePump
{
    Task RunAsync(CancellationToken ct);
}