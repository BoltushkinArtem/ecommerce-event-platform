namespace Messaging.Kafka.Topics;

public interface IKafkaTopicResolver
{
    string Resolve<TEvent>();
    string Resolve(Type eventType);
}