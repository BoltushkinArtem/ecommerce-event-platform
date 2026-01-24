namespace Messaging.Kafka.Configuration;

public interface IKafkaTopicsOptions
{
    public IReadOnlyCollection<KafkaTopicOptions> Topics { get; }
}