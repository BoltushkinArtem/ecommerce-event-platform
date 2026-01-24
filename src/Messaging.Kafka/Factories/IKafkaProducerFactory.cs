using Confluent.Kafka;

namespace Messaging.Kafka.Factories;

public interface IKafkaProducerFactory
{
    IProducer<string, string>  Create();
}