namespace Messaging.Abstractions;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(
        string key,
        T message,
        CancellationToken ct = default);
}