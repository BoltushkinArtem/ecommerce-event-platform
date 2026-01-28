namespace Messaging.Kafka.Core.Configuration;

public record KafkaHandlerOptions(Type EventType, Type HandlerType);