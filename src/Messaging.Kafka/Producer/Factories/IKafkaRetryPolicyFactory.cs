using Polly;

namespace Messaging.Kafka.Producer.Factories;

public interface IKafkaRetryPolicyFactory
{
    IAsyncPolicy Create();
}