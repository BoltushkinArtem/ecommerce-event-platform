using System.Reflection;
using Messaging.Abstractions.Messages;

namespace Messaging.Kafka.Core.Attributes;

public class EventKeyResolver: IEventKeyResolver
{
    public string Resolve<TEvent>() => Resolve(typeof(TEvent));

    public string Resolve(Type eventType)
        => eventType.GetCustomAttribute<EventKeyAttribute>()?.Value
           ?? throw new InvalidOperationException(
               $"EventContractKeyResolver not found on {eventType.FullName}");
}