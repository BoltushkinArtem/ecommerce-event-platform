using Confluent.Kafka;

namespace Messaging.Kafka.Configuration;

public sealed class KafkaProducerOptions
{
    public Acks Acks { get; init; } = Acks.All;
    public bool EnableIdempotence { get; init; } = true;
}