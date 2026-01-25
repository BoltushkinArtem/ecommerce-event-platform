namespace Messaging.Abstractions;

public interface IKafkaMessageHandler<in TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken ct);
}