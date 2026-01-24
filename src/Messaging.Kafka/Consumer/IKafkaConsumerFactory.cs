using Confluent.Kafka;

namespace Messaging.Kafka.Consumer;

public interface IKafkaConsumerFactory
{
    IConsumer<string, string> Create();
}