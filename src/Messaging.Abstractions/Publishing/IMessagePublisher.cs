namespace Messaging.Abstractions.Publishing;

public interface IMessagePublisher
{
    Task ProduceAsync<T>(
        string key,
        T message,
        CancellationToken ct = default);
}