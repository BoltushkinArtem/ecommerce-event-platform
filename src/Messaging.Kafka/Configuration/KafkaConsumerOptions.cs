namespace Messaging.Kafka.Configuration;

public sealed class KafkaConsumerOptions
{
    public string GroupId  { get; init; } = string.Empty;
    public bool EnableAutoCommit { get; init; } = false;
}