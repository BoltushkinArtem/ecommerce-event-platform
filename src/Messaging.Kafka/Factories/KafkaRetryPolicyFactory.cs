using Confluent.Kafka;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Options;
using Polly;

namespace Messaging.Kafka.Factories;

public sealed class KafkaRetryPolicyFactory(IOptions<KafkaOptions> options) : IKafkaRetryPolicyFactory
{
    private readonly KafkaRetryOptions _options = options.Value.Retry;

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