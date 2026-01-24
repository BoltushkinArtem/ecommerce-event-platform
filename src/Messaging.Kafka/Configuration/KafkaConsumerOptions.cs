namespace Messaging.Kafka.Configuration;

public sealed class KafkaConsumerOptions
{
    public IReadOnlyCollection<KafkaTopicOptions> Topics { get; init; } = [];
    public string GroupId  { get; init; } = string.Empty;
    public int MaxDegreeOfParallelism  { get; init; } = 5;
    public bool EnableAutoCommit { get; init; } = false;
}