namespace Messaging.Kafka.Configuration;

public record KafkaHandlerOptions(Type EventType, Type HandlerType);