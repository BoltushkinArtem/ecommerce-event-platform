using System.Reflection;
using Messaging.Abstractions;

namespace Messaging.Kafka.Attributes;

public class EventContractKeyResolver: IEventContractKeyResolver
{
    public string Resolve<TEvent>() => Resolve(typeof(TEvent));

    public string Resolve(Type eventType)
        => eventType.GetCustomAttribute<EventContractAttribute>()?.Key
           ?? throw new InvalidOperationException(
               $"EventContractKeyResolver not found on {eventType.FullName}");
}