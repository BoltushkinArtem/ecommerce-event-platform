namespace Messaging.Kafka.Serialization;

public interface IKafkaMessageSerializer
{
    string Serialize<T>(T message);
    object Deserialize(string payload, Type messageType);
}