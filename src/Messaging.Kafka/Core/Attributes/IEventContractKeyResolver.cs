namespace Messaging.Kafka.Core.Attributes;

public interface IEventContractKeyResolver
{
    string Resolve<TEvent>();
    string Resolve(Type eventType);
}