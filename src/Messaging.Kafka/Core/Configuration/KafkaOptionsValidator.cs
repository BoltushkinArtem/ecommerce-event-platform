using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Core.Configuration;

public sealed class KafkaOptionsValidator
    : IValidateOptions<KafkaOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        KafkaOptions options)
    {
        var failures = new List<string>();

        ValidateBootstrapServers(options.BootstrapServers, failures);
        ValidateRetryOptions(options.Retry, failures);
        ValidateTopics(options.Topics, failures);

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }

    private static void ValidateBootstrapServers(
        string bootstrapServers,
        ICollection<string> failures)
    {
        if (string.IsNullOrWhiteSpace(bootstrapServers))
        {
            failures.Add("Kafka:BootstrapServers is required.");
            return;
        }

        var servers = bootstrapServers.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var server in servers)
        {
            if (!server.Contains(':'))
            {
                failures.Add(
                    $"Kafka:BootstrapServers entry '{server}' must be in 'host:port' format.");
            }
        }
    }

    private static void ValidateRetryOptions(
        KafkaRetryOptions retry,
        ICollection<string> failures)
    {
        if (retry.RetryCount < 0)
        {
            failures.Add("Kafka:Retry:RetryCount must be >= 0.");
        }

        if (retry.BaseDelayMs <= 0)
        {
            failures.Add("Kafka:Retry:BaseDelayMs must be greater than 0.");
        }
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
            .GroupBy(t => t.EventKey)
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
            if (string.IsNullOrWhiteSpace(topic.EventKey))
            {
                failures.Add("Kafka:Topics[].Event must be specified.");
            }

            if (string.IsNullOrWhiteSpace(topic.Name))
            {
                failures.Add(
                    $"Kafka:Topics[].Name must be specified for event '{topic.EventKey}'.");
            }
        }
    }
}