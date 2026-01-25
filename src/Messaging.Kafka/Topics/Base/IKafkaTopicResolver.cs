namespace Messaging.Kafka.Topics.Base;

public interface IKafkaTopicResolver
{
    string Resolve<TEvent>();
}