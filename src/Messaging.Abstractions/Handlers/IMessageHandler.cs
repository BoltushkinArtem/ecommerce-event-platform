namespace Messaging.Abstractions.Handlers;

public interface IMessageHandler<in TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken ct);
}