using Messaging.Kafka.Configuration;

namespace Messaging.Kafka.Registry;

public sealed class KafkaHandlerRegistryOptions
{
    public IList<KafkaHandlerOptions> Handlers { get; } = new List<KafkaHandlerOptions>();
}