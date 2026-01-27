using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Topics;

public class KafkaTopicResolver : IKafkaTopicResolver
{
    private readonly IReadOnlyDictionary<string, KafkaTopicDefinition> _topics;
    private readonly ILogger<KafkaTopicResolver> _logger;

    public KafkaTopicResolver(
        IOptions<KafkaOptions> options,
        ILogger<KafkaTopicResolver> logger)
    {
        _logger = logger;

        _topics = options.Value.Topics.ToDictionary(
            t => t.Event,
            t => new KafkaTopicDefinition(t.Event, t.Name));

        _logger.LogInformation(
            "Kafka topic resolver initialized. Events -> Topics: {Topics}",
            _topics.Select(x => $"{x.Key} -> {x.Value.Name}"));
    }

    private string Resolve(string eventName)
    {
        if (_topics.TryGetValue(eventName, out var topic)) return topic.Name;
        _logger.LogError(
            "Kafka topic not configured for event {Event}. Available events: {Events}",
            eventName,
            _topics.Keys);

        throw new InvalidOperationException(
            $"Kafka topic not configured for event '{eventName}'.");
    }
    

    public string Resolve<TEvent>() => Resolve(typeof(TEvent).Name);
    
    public string Resolve(Type eventType)=> Resolve(eventType.Name);
}