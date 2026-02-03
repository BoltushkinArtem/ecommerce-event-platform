using Confluent.Kafka;

namespace Messaging.Kafka.Consumer.Pipeline;

public record KafkaConsumeContext(
    ConsumeResult<string, string> ConsumeResult,
    object? Message,
    Type? MessageType
);