namespace Messaging.Abstractions;

public interface IMessagePublisher
{
    Task ProduceAsync<T>(
        string key,
        T message,
        CancellationToken ct = default);
}