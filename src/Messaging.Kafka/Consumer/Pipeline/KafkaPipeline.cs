using Confluent.Kafka;
using Messaging.Kafka.Consumer.Pipeline.Steps;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pipeline;

public sealed class KafkaPipeline(
    IEnumerable<IKafkaPipelineStep> steps,
    ILogger<KafkaPipeline> logger)
    : IKafkaPipeline
{
    private readonly IReadOnlyList<IKafkaPipelineStep> _steps = steps.ToList();

    public async Task<PipelineExecutionResult> ExecuteAsync(
        ConsumeResult<string, string> result,
        CancellationToken ct)
    {
        KafkaConsumeContext context = new(result,null, null,null,false);

        foreach (var step in _steps)
        {
            try
            {
                context = await step.ExecuteAsync(context, ct);

                if (context.Exception is not null)
                {
                    logger.LogWarning(
                        "Pipeline interrupted at step {Step}",
                        step.GetType().Name);

                    return PipelineExecutionResult.Failure(context.Exception);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unhandled exception in pipeline step {Step}",
                    step.GetType().Name);

                return PipelineExecutionResult.Failure(ex);
            }
        }

        return context.IsHandled
            ? PipelineExecutionResult.Success()
            : PipelineExecutionResult.Failure(
                new InvalidOperationException("Message was not handled"));
    }
}