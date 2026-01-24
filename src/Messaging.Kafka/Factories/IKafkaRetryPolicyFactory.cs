using Polly;

namespace Messaging.Kafka.Factories;

public interface IKafkaRetryPolicyFactory
{
    IAsyncPolicy Create();
}