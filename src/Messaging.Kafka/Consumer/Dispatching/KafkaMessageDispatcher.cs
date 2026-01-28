using Confluent.Kafka;
using Messaging.Kafka.Consumer.Registry;
using Messaging.Kafka.Core.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Consumer.Dispatching;

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