
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Registry;

public sealed class KafkaHandlerRegistry : IKafkaHandlerRegistry
{
    private readonly Dictionary<string, KafkaHandlerDescriptor> _byTopic;

    public KafkaHandlerRegistry(IOptions<KafkaHandlerRegistryOptions> options)
    {
        _byTopic = options.Value.Handlers.ToDictionary(d => d.Topic);
    }

    public KafkaHandlerDescriptor GetDescriptor(string topic)
        => _byTopic[topic];
    
    public IReadOnlyCollection<string> Topics => _byTopic.Keys;
}