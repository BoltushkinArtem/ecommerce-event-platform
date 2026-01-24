namespace Messaging.Abstractions.Handlers;

public interface IKafkaMessageHandler<in TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken ct);
}