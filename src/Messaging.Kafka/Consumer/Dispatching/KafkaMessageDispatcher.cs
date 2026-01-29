using Confluent.Kafka;
using Messaging.Kafka.Consumer.Invocation;
using Messaging.Kafka.Consumer.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Consumer.Dispatching;

public sealed class KafkaMessageDispatcher(
    IServiceScopeFactory scopeFactory,
    IKafkaHandlerRegistry registry,
    IKafkaHandlerInvoker invoker)
    : IKafkaMessageDispatcher
{
    public async Task DispatchAsync(
        ConsumeResult<string, string> result,
        CancellationToken ct)
    {
        var descriptor = registry.GetDescriptor(result.Topic);

        using var scope = scopeFactory.CreateScope();

        await invoker.InvokeAsync(
            scope.ServiceProvider,
            descriptor.EventType,
            descriptor.HandlerType,
            result.Message.Value,
            ct);
    }
}