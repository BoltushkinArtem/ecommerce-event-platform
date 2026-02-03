using Confluent.Kafka;
using Messaging.Kafka.Consumer.Pipeline.Steps;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pipeline;

public sealed class KafkaPipeline(
    IEnumerable<IKafkaPipelineStep> steps,
    ILogger<KafkaPipeline> logger)
    : IKafkaPipeline
{
    private readonly IReadOnlyList<IKafkaPipelineStep> _steps =
        steps.ToList();

    public async Task<KafkaProcessingResult> ExecuteAsync(
        ConsumeResult<string, string> result,
        CancellationToken ct)
    {
        KafkaConsumeContext context = new(
            ConsumeResult: result,
            Message: null!,
            MessageType: null!);

        try
        {
            foreach (var step in _steps)
            {
                context = await step.ExecuteAsync(context, ct);
            }

            return KafkaProcessingResult.Commit();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Validation error");
            return KafkaProcessingResult.Skip(ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled processing error");
            return KafkaProcessingResult.Retry(ex);
        }
    }
}