namespace Messaging.Kafka.Core.Topics;

public interface IKafkaTopicResolver
{
    string Resolve<TEvent>();
    string Resolve(Type eventType);
}