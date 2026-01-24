using Confluent.Kafka;

namespace Messaging.Kafka.Configuration;

public sealed class KafkaProducerOptions
{
    public IReadOnlyCollection<KafkaTopicOptions> Topics { get; init; } = [];
    public Acks Acks { get; init; } = Acks.All;
    public bool EnableIdempotence { get; init; } = true;
}