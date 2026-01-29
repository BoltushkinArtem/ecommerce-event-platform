namespace Messaging.Kafka.Core.Attributes;

public interface IEventKeyResolver
{
    string Resolve<TEvent>();
    string Resolve(Type eventType);
}