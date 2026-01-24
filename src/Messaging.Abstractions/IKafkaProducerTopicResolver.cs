namespace Messaging.Abstractions;

public interface IKafkaProducerTopicResolver
{
    string Resolve<TEvent>();
}