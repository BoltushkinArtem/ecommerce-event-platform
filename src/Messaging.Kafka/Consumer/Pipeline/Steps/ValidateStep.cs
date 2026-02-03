using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pipeline.Steps;

public sealed class ValidateStep(
    ILogger<ValidateStep> logger)
    : IKafkaPipelineStep
{
    public Task<KafkaConsumeContext> ExecuteAsync(
        KafkaConsumeContext context,
        CancellationToken ct)
    {
        if (context.Message is not null) return Task.FromResult(context);
        logger.LogError("Validation failed: Message is null");
        throw new InvalidOperationException("Message is null");
    }
}