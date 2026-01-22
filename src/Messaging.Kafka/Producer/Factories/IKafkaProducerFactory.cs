using Confluent.Kafka;

namespace Messaging.Kafka.Producer.Factories;

public interface IKafkaProducerFactory
{
    IProducer<string, string>  Create();
}