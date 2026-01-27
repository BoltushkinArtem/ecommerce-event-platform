namespace Messaging.Kafka.Registry;

public sealed record KafkaHandlerDescriptor(
    string Topic,
    Func<IServiceProvider, string, CancellationToken, Task> InvokeAsync
);