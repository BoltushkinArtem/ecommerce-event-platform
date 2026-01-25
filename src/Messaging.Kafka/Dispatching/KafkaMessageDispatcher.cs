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
        var message = serializer.Deserialize(
            result.Message.Value,
            descriptor.EventType);

        using var scope = scopeFactory.CreateScope();

        var handler = scope.ServiceProvider
            .GetRequiredService(descriptor.HandlerType);

        await ((dynamic)handler)
            .HandleAsync((dynamic)message, ct);
    }
}