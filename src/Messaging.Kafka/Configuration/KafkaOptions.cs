namespace Messaging.Kafka.Configuration;

public sealed record KafkaOptions
{
    public required string BootstrapServers { get; init; }
    public required KafkaRetryOptions Retry { get; init; }
    
}