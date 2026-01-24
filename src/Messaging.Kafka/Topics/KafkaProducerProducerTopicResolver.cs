using Messaging.Abstractions;
using Messaging.Abstractions.Metadata;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Topics;

public sealed class KafkaProducerProducerTopicResolver : IKafkaProducerTopicResolver
{
    private readonly IReadOnlyDictionary<string, KafkaProducerTopicDefinition> _topicsByEvent;
    private readonly ILogger<KafkaConsumerTopicResolver> _logger;

    public KafkaProducerProducerTopicResolver(
        ILogger<KafkaConsumerTopicResolver> logger,
        IOptions<KafkaProducerOptions> options)
    {
        _logger = logger;

        _topicsByEvent = options.Value.Topics.ToDictionary(
            t => t.Event,
            t => new KafkaProducerTopicDefinition(t.Event, t.Name));

        _logger.LogInformation(
            "Kafka topic resolver initialized. Events -> Topics: {Topics}",
            _topicsByEvent.Select(t => $"{t.Key} -> {t.Value.Name}"));
    }

    public string Resolve<TEvent>()
    {
        var eventName = typeof(TEvent).Name;

        if (!_topicsByEvent.TryGetValue(eventName, out var topic))
        {
            _logger.LogError(
                "Kafka topic not configured for event {Event}. Available events: {AvailableEvents}",
                eventName,
                _topicsByEvent.Keys);

            throw new InvalidOperationException($"Kafka topic not configured for event '{eventName}'.");
        }

        return topic.Name;
    }
}