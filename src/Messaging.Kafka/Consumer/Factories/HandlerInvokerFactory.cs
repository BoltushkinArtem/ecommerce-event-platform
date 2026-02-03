using System.Collections.Concurrent;
using System.Reflection;
using Messaging.Abstractions.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Consumer.Factories;

public sealed class HandlerInvokerFactory: IHandlerInvokerFactory
{
    private readonly ConcurrentDictionary<(Type, Type),
        Func<IServiceProvider, object, CancellationToken, Task>> _cache = new();

    public Func<IServiceProvider, object, CancellationToken, Task>
        GetOrCreate(Type eventType, Type handlerType)
    {
        return _cache.GetOrAdd(
            (eventType, handlerType),
            static key => BuildInvoker(key.Item1, key.Item2));
    }

    private static Func<IServiceProvider, object, CancellationToken, Task>
        BuildInvoker(Type eventType, Type handlerType)
    {
        var method = typeof(HandlerInvokerFactory)
            .GetMethod(nameof(CreateInvoker), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(eventType, handlerType);

        return (Func<IServiceProvider, object, CancellationToken, Task>)
            method.Invoke(null, null)!;
    }

    private static Func<IServiceProvider, object, CancellationToken, Task>
        CreateInvoker<TEvent, THandler>()
        where TEvent : class
        where THandler : IMessageHandler<TEvent>
    {
        return static async (sp, message, ct) =>
        {
            var handler = sp.GetRequiredService<THandler>();
            await handler.HandleAsync((TEvent)message, ct);
        };
    }
}