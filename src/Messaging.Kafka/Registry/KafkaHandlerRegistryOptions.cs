namespace Messaging.Kafka.Registry;

public sealed class KafkaHandlerRegistryOptions
{
    public IList<KafkaHandlerDescriptor> Handlers { get; } = new List<KafkaHandlerDescriptor>();
}