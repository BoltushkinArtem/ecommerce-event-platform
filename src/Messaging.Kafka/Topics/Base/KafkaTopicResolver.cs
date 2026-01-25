using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Topics.Base;

public class KafkaTopicResolver<TOptions> : IKafkaTopicResolver
    where TOptions : class, IKafkaTopicsOptions
{
    private readonly IReadOnlyDictionary<string, KafkaTopicDefinition> _topics;
    private readonly ILogger<KafkaTopicResolver<TOptions>> _logger;

    protected KafkaTopicResolver(
        IOptions<TOptions> options,
        ILogger<KafkaTopicResolver<TOptions>> logger)
    {
        _logger = logger;

        _topics = options.Value.Topics.ToDictionary(
            t => t.Event,
            t => new KafkaTopicDefinition(t.Event, t.Name));

        _logger.LogInformation(
            "Kafka topic resolver initialized for {OptionsType}. Events -> Topics: {Topics}",
            typeof(TOptions).Name,
            _topics.Select(x => $"{x.Key} -> {x.Value.Name}"));
    }

    public string Resolve<TEvent>()
    {
        var eventName = typeof(TEvent).Name;

        if (!_topics.TryGetValue(eventName, out var topic))
        {
            _logger.LogError(
                "Kafka topic not configured for event {Event}. Available events: {Events}",
                eventName,
                _topics.Keys);

            throw new InvalidOperationException(
                $"Kafka topic not configured for event '{eventName}'.");
        }

        return topic.Name;
    }
}