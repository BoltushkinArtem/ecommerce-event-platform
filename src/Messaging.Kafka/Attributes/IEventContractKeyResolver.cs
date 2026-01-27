namespace Messaging.Kafka.Attributes;

public interface IEventContractKeyResolver
{
    string Resolve<TEvent>();
    string Resolve(Type eventType);
}