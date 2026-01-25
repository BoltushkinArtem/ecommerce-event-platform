using Confluent.Kafka;

namespace Messaging.Kafka.Dispatching;

public interface IKafkaMessageDispatcher
{
    Task DispatchAsync(
        ConsumeResult<string, string> result,
        CancellationToken ct);
}