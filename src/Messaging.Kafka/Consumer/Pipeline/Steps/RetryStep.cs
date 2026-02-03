using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pipeline.Steps;

public sealed class RetryStep(
    ILogger<RetryStep> logger)
    : IKafkaPipelineStep
{
    public Task<KafkaConsumeContext> ExecuteAsync(
        KafkaConsumeContext context,
        CancellationToken ct)
    {
        if (context.Exception is not null)
        {
            logger.LogError(
                context.Exception,
                "Message processing failed. Offset={Offset}",
                context.ConsumeResult.Offset.Value);
        }

        return Task.FromResult(context);
    }
}