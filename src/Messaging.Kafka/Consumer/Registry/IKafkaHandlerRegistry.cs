namespace Messaging.Kafka.Consumer.Registry;

public interface IKafkaHandlerRegistry
{
    IReadOnlyCollection<string> Topics { get; }
    KafkaHandlerDescriptor GetDescriptor(string topic);
}