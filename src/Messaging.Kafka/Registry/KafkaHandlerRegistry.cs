using Messaging.Kafka.Topics;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Registry;

public sealed class KafkaHandlerRegistry : IKafkaHandlerRegistry
{
    private readonly Dictionary<string, KafkaHandlerDescriptor> _byTopic;

    public KafkaHandlerRegistry(
        IOptions<KafkaHandlerRegistryOptions> options,
        IKafkaTopicResolver topicResolver)
    {
        var dictionary = new Dictionary<string, KafkaHandlerDescriptor>(
            options.Value.Handlers.Count);

        foreach (var d in options.Value.Handlers)
        {
            var topic = topicResolver.Resolve(d.EventType);

            dictionary.Add(
                topic,
                new KafkaHandlerDescriptor(
                    topic,
                    d.EventType,
                    d.HandlerType));
        }

        _byTopic = dictionary;
    }

    public KafkaHandlerDescriptor GetDescriptor(string topic)
        => _byTopic[topic];

    public IReadOnlyCollection<string> Topics => _byTopic.Keys;
}