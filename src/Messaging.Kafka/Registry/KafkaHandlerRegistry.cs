
namespace Messaging.Kafka.Registry;

public sealed class KafkaHandlerRegistry : IKafkaHandlerRegistry
{
    private readonly Dictionary<string, KafkaHandlerDescriptor> _byTopic;

    public KafkaHandlerRegistry(IEnumerable<KafkaHandlerDescriptor> descriptors)
    {
        _byTopic = descriptors.ToDictionary(d => d.Topic);
    }

    public KafkaHandlerDescriptor GetDescriptor(string topic)
        => _byTopic[topic];
    
    public IReadOnlyCollection<string> Topics => _byTopic.Keys;
}