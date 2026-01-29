using System.Linq.Expressions;
using Messaging.Abstractions.Handlers;
using Messaging.Kafka.Core.Serialization;
using Messaging.Kafka.Core.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Consumer.Registry;

public sealed class KafkaHandlerRegistry : IKafkaHandlerRegistry
{
    private readonly Dictionary<string, KafkaHandlerDescriptor> _byTopic;

    public KafkaHandlerRegistry(
        IOptions<KafkaHandlerRegistryOptions> options,
        IKafkaTopicResolver topicResolver,
        IKafkaMessageSerializer serializer)
    {
        _byTopic = new Dictionary<string, KafkaHandlerDescriptor>(
            options.Value.Handlers.Count);

        foreach (var handler  in options.Value.Handlers)
        {
            var topic = topicResolver.Resolve(handler.EventType);

            if (_byTopic.ContainsKey(topic))
            {
                throw new InvalidOperationException(
                    $"Duplicate Kafka handler for topic '{topic}'. Each topic must have exactly one handler.");
            }

            _byTopic.Add(
                topic,
                new KafkaHandlerDescriptor(
                    Topic: topic,
                    EventType: handler.EventType,
                    HandlerType: handler.HandlerType));
        }
    }

    public KafkaHandlerDescriptor GetDescriptor(string topic)
    {
        if (!_byTopic.TryGetValue(topic, out var descriptor))
        {
            throw new InvalidOperationException(
                $"No Kafka handler registered for topic '{topic}'. " +
                $"Available topics: {string.Join(", ", _byTopic.Keys)}");
        }

        return descriptor;
    }

    public IReadOnlyCollection<string> Topics => _byTopic.Keys;
}