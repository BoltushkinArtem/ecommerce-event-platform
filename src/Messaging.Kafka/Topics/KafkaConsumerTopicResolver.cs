using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Topics;

public sealed class KafkaConsumerTopicResolver: IKafkaConsumerTopicResolver
{
    private readonly Dictionary<string, string> _map;
    private readonly ILogger<KafkaConsumerTopicResolver> _logger;

    public KafkaConsumerTopicResolver(
        ILogger<KafkaConsumerTopicResolver> logger,
        IOptions<KafkaConsumerOptions> options)
    {
        _logger = logger;

        _map = options.Value.Topics.ToDictionary(
            x => x.Event,
            x => x.Name);

        _logger.LogInformation(
            "Kafka topic resolver initialized. Events -> Topics: {Topics}",
            _map.Select(t => $"{t.Key} -> {t.Value}"));
    }

    public string Resolve<TEvent>()
    {
        var eventName = typeof(TEvent).Name;
        
        if (!_map.TryGetValue(eventName, out var topic))
            throw new InvalidOperationException(
                $"Topic not configured for event {eventName}");

        return topic;
    }
}