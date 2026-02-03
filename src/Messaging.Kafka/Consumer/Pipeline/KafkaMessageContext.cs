using Confluent.Kafka;

namespace Messaging.Kafka.Consumer.Pipeline;

public sealed class KafkaMessageContext
{
    public required ConsumeResult<string, string> ConsumeResult { get; init; }

    public object? Message { get; set; }
    public Type? MessageType { get; set; }
    public Exception? Exception { get; set; }

    public bool IsHandled { get; set; }
}