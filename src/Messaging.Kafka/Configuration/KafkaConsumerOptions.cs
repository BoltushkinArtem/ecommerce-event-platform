namespace Messaging.Kafka.Configuration;

public sealed class KafkaConsumerOptions : IKafkaTopicsOptions
{
    public IReadOnlyCollection<KafkaTopicOptions> Topics { get; init; } = [];
    public string GroupId  { get; init; } = string.Empty;
    public bool EnableAutoCommit { get; init; } = false;
}