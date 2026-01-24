using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Configuration;

public sealed class KafkaConsumerOptionsValidator: IValidateOptions<KafkaConsumerOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        KafkaConsumerOptions options)
    {
        var failures = new List<string>();
        
        ValidateTopics(options.Topics, failures);
        
        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
    
    private static void ValidateTopics(
        IReadOnlyCollection<KafkaTopicOptions> topics,
        ICollection<string> failures)
    {
        if (topics.Count == 0)
        {
            failures.Add("Kafka:Topics must contain at least one topic mapping.");
            return;
        }

        var duplicateEvents = topics
            .GroupBy(t => t.Event)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (duplicateEvents.Length > 0)
        {
            failures.Add(
                $"Kafka:Topics contains duplicate event mappings: {string.Join(", ", duplicateEvents)}");
        }

        foreach (var topic in topics)
        {
            if (string.IsNullOrWhiteSpace(topic.Event))
            {
                failures.Add("Kafka:Topics[].Event must be specified.");
            }

            if (string.IsNullOrWhiteSpace(topic.Name))
            {
                failures.Add(
                    $"Kafka:Topics[].Name must be specified for event '{topic.Event}'.");
            }
        }
    }
}
