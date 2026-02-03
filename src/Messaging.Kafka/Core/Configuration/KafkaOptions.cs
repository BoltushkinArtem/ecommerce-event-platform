namespace Messaging.Kafka.Core.Configuration;

public sealed record KafkaOptions
{
    public required string BootstrapServers { get; init; }
    public required KafkaRetryOptions Retry { get; init; }
    public IReadOnlyList<KafkaTopicOptions> Topics { get; init; } = [];
}