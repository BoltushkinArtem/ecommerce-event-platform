using Messaging.Kafka.Consumer.Registry;
using Messaging.Kafka.Core.Serialization;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pipeline.Steps;

public sealed class DeserializeStep(
    IKafkaHandlerRegistry registry,
    IKafkaMessageSerializer serializer,
    ILogger<DeserializeStep> logger)
    : IKafkaPipelineStep
{
    public Task<KafkaConsumeContext> ExecuteAsync(
        KafkaConsumeContext context,
        CancellationToken ct)
    {
        var topic = context.ConsumeResult.Topic;
        var descriptor = registry.GetDescriptor(topic);

        logger.LogInformation(
            "Deserializing message. Topic={Topic}, EventType={EventType}",
            topic,
            descriptor.EventType.Name);
        
        return Task.FromResult(
            context with
            {
                Message = serializer.Deserialize(
                    context.ConsumeResult.Message.Value,
                    descriptor.EventType),
                MessageType = descriptor.EventType
            });
    }
}