namespace Messaging.Abstractions.Registry;

public sealed record KafkaHandlerDescriptor(
    string Topic,
    Type EventType,
    Type HandlerType
);