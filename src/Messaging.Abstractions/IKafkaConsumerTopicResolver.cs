namespace Messaging.Abstractions;

public interface IKafkaConsumerTopicResolver
{
    string Resolve<TEvent>();
}