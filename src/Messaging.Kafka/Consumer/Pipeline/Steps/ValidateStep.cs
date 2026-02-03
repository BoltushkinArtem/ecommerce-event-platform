using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pipeline.Steps;

public sealed class ValidateStep(
    ILogger<ValidateStep> logger)
    : IKafkaPipelineStep
{
    public Task ExecuteAsync(
        KafkaMessageContext context,
        CancellationToken ct)
    {
        if (context.Message is not null) return Task.CompletedTask;
        context.Exception = new InvalidOperationException("Message is null");
        logger.LogError("Validation failed: Message is null");

        return Task.CompletedTask;
    }
}