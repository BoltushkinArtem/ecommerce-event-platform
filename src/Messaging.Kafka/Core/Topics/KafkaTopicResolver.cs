using Messaging.Kafka.Core.Attributes;
using Messaging.Kafka.Core.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Core.Topics;

public class KafkaTopicResolver : IKafkaTopicResolver
{
    private readonly IReadOnlyDictionary<string, KafkaTopicDefinition> _topics;
    private readonly ILogger<KafkaTopicResolver> _logger;
    private readonly IEventKeyResolver  _eventKeyResolver;

    public KafkaTopicResolver(
        IOptions<KafkaOptions> options,
        ILogger<KafkaTopicResolver> logger,
        IEventKeyResolver  eventKeyResolver)
    {
        _logger = logger;
        _eventKeyResolver = eventKeyResolver;
        
        _topics = options.Value.Topics.ToDictionary(
            t => t.Event,
            t => new KafkaTopicDefinition(t.Event, t.Name));

        _logger.LogInformation(
            "Kafka topic resolver initialized. Events -> Topics: {Topics}",
            _topics.Select(x => $"{x.Key} -> {x.Value.Name}"));
    }

    public string Resolve<TEvent>() => Resolve(typeof(TEvent));

    public string Resolve(Type eventType)
    {
        var eventKey = _eventKeyResolver.Resolve(eventType);
        if (_topics.TryGetValue(eventKey, out var topic)) return topic.Name;
        _logger.LogError(
            "Kafka topic not configured for event {Event}. Available events: {Events}",
            eventKey,
            _topics.Keys);

        throw new InvalidOperationException(
            $"Kafka topic not configured for event '{eventKey}'.");
    }
}