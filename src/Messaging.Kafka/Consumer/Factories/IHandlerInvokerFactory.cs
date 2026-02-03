namespace Messaging.Kafka.Consumer.Factories;

public interface IHandlerInvokerFactory
{
    Func<IServiceProvider, object, CancellationToken, Task>
        GetOrCreate(Type eventType, Type handlerType);
}