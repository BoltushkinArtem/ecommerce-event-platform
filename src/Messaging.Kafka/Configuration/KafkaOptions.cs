namespace Messaging.Kafka.Configuration;

public sealed record KafkaOptions
{
    public required string BootstrapServers { get; init; }
    public IReadOnlyCollection<KafkaTopicOptions> Topics { get; init; } = [];
    public KafkaRetryOptions Retry { get; init; }
}