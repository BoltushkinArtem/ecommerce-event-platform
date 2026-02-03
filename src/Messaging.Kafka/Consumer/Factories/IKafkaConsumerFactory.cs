using Confluent.Kafka;

namespace Messaging.Kafka.Consumer.Factories;

public interface IKafkaConsumerFactory
{
    IConsumer<string, string> Create();
}