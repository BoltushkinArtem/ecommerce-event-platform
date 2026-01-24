namespace Messaging.Kafka.DependencyInjection;

public enum KafkaMessagingMode
{
    OnlyProducer,
    OnlyConsumer,
    ProducerAndConsumer
}