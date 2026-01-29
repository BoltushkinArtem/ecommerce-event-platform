using Messaging.Abstractions.Handlers;
using Messaging.Kafka.Core.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Consumer.Invocation;

public sealed class KafkaHandlerInvoker(
    IKafkaMessageSerializer serializer)
    : IKafkaHandlerInvoker
{
    public async Task InvokeAsync(
        IServiceProvider sp,
        Type eventType,
        Type handlerType,
        string payload,
        CancellationToken ct)
    {
        var message = serializer.Deserialize(payload, eventType);

        var handler = sp.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(
            nameof(IMessageHandler<object>.HandleAsync),
            new[] { eventType, typeof(CancellationToken) });

        if (method is null)
        {
            throw new InvalidOperationException(
                $"Handler {handlerType.Name} does not handle {eventType.Name}");
        }

        var task = (Task)method.Invoke(handler, new[] { message, ct })!;
        await task.ConfigureAwait(false);
    }
}