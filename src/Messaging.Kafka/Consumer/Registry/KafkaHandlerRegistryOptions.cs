using Messaging.Kafka.Core.Configuration;

namespace Messaging.Kafka.Consumer.Registry;

public sealed class KafkaHandlerRegistryOptions
{
    public IList<KafkaHandlerOptions> Handlers { get; } = new List<KafkaHandlerOptions>();
}