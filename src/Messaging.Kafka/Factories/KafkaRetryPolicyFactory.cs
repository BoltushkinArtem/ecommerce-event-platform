using Confluent.Kafka;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Options;
using Polly;

namespace Messaging.Kafka.Factories;

public sealed class KafkaRetryPolicyFactory : IKafkaRetryPolicyFactory
{
    private readonly KafkaRetryOptions _options;

    public KafkaRetryPolicyFactory(IOptions<KafkaOptions> options)
    {
        _options = options.Value.Retry;
    }

    public IAsyncPolicy Create()
    {
        return Policy
            .Handle<ProduceException<string, string>>(ex => !ex.Error.IsFatal)
            .WaitAndRetryAsync(
                retryCount: _options.RetryCount,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromMilliseconds(
                        _options.BaseDelayMs * attempt));
    }
}