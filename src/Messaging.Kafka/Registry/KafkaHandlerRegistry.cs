using Messaging.Abstractions;
using Messaging.Abstractions.Metadata;
using Messaging.Abstractions.Registry;

namespace Messaging.Kafka.Registry;

public sealed class KafkaHandlerRegistry : IKafkaHandlerRegistry
{
    private readonly Dictionary<string, KafkaHandlerDescriptor> _byTopic;

    public KafkaHandlerRegistry(IEnumerable<KafkaHandlerDescriptor> descriptors)
    {
        _byTopic = descriptors.ToDictionary(d => d.Topic);
    }

    /*public KafkaConsumerTopicDefinition GetByTopic(string topic)
    {
        if (!_byTopic.TryGetValue(topic, out var d))
            throw new InvalidOperationException($"No handler for topic {topic}");

        return new KafkaConsumerTopicDefinition(d.EventType, d.Topic);
    }*/

    public KafkaHandlerDescriptor GetDescriptor(string topic)
        => _byTopic[topic];
    
    public IReadOnlyCollection<string> Topics => _byTopic.Keys;
}