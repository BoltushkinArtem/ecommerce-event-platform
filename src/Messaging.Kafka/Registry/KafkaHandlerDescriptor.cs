namespace Messaging.Kafka.Registry;

public sealed record KafkaHandlerDescriptor(
    string Topic,
    Type EventType,
    Type HandlerType
);