namespace Messaging.Abstractions;

public interface IKafkaTopicResolver
{
    string Resolve<TEvent>();
}