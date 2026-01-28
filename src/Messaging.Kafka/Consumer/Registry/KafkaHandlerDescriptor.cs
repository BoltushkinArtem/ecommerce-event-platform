namespace Messaging.Kafka.Consumer.Registry;

public sealed record KafkaHandlerDescriptor(
    string Topic,
    Func<IServiceProvider, string, CancellationToken, Task> InvokeAsync
);