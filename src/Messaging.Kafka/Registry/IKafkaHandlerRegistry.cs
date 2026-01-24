namespace Messaging.Kafka.Registry;

public interface IKafkaHandlerRegistry
{
    IReadOnlyCollection<string> Topics { get; }
    KafkaHandlerDescriptor GetDescriptor(string topic);
}