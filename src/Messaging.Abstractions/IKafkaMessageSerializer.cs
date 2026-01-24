namespace Messaging.Abstractions;

public interface IKafkaMessageSerializer
{
    string Serialize<T>(T message);
    object Deserialize(string payload, Type messageType);
}