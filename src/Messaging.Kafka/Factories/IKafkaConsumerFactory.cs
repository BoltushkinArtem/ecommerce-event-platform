using Confluent.Kafka;

namespace Messaging.Kafka.Factories;

public interface IKafkaConsumerFactory
{
    IConsumer<string, string> Create();
}