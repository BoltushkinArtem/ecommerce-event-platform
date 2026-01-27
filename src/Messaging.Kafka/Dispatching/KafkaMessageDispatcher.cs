using Confluent.Kafka;
using Messaging.Abstractions;
using Messaging.Kafka.Registry;
using Messaging.Kafka.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Dispatching;

public sealed class KafkaMessageDispatcher(
    IServiceScopeFactory scopeFactory,
    IKafkaHandlerRegistry registry,
    IKafkaMessageSerializer serializer)
    : IKafkaMessageDispatcher
{
    public async Task DispatchAsync(
        ConsumeResult<string, string> result,
        CancellationToken ct)
    {
        var descriptor = registry.GetDescriptor(result.Topic);

        using var scope = scopeFactory.CreateScope();

        await descriptor.InvokeAsync(
            scope.ServiceProvider,
            result.Message.Value,
            ct);
    }
}