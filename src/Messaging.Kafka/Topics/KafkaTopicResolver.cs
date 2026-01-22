using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Topics;

public sealed class KafkaTopicResolver : IKafkaTopicResolver
{
    private readonly IReadOnlyDictionary<string, KafkaTopicDefinition> _topics;
    private readonly ILogger<KafkaTopicResolver> _logger;

    public KafkaTopicResolver(
        ILogger<KafkaTopicResolver> logger,
        IOptions<KafkaOptions> options)
    {
        _logger = logger;

        _topics = options.Value.Topics.ToDictionary(
            t => t.Event,
            t => new KafkaTopicDefinition(t.Name));

        _logger.LogInformation(
            "Kafka topic resolver initialized. Configured topics: {Topics}",
            _topics.Select(t => $"{t.Key} -> {t.Value.Name}"));
    }

    public string Resolve<TEvent>()
    {
        var eventName = typeof(TEvent).Name;

        if (!_topics.TryGetValue(eventName, out var topic))
        {
            _logger.LogError(
                "Kafka topic not configured for event {Event}. Available events: {AvailableEvents}",
                eventName,
                _topics.Keys);

            throw new InvalidOperationException(
                $"Kafka topic not configured for event '{eventName}'.");
        }

        _logger.LogDebug(
            "Resolved Kafka topic for event {Event}. Topic={Topic}",
            eventName,
            topic.Name);

        return topic.Name;
    }

    public IReadOnlyCollection<KafkaTopicDefinition> GetAllTopics()
        => _topics.Values.ToArray();
}