namespace Messaging.Kafka.Configuration;

public sealed class KafkaRetryOptions
{
    public int RetryCount { get; init; } = 3;
    public int BaseDelayMs { get; init; } = 200;
}