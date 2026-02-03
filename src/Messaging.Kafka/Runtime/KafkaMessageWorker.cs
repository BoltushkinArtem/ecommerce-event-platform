using Messaging.Abstractions.Runtime;
using Messaging.Kafka.Consumer.Pump;

namespace Messaging.Kafka.Runtime;

internal sealed class KafkaMessageWorker(IKafkaMessagePump pump) : IMessageWorker
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        return pump.RunAsync(cancellationToken);
    }
}