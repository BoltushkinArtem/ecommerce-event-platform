namespace Messaging.Kafka.Consumer.Invocation;

public interface IKafkaHandlerInvoker
{
    Task InvokeAsync(
        IServiceProvider sp,
        Type eventType,
        Type handlerType,
        string payload,
        CancellationToken ct);
}