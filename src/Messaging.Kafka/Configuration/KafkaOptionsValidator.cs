using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Configuration;

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
}