namespace Messaging.Kafka.Consumer.Pipeline;

public sealed class PipelineExecutionResult
{
    public bool IsHandled { get; init; }
    public Exception? Exception { get; init; }

    public static PipelineExecutionResult Success()
        => new() { IsHandled = true };

    public static PipelineExecutionResult Failure(Exception ex)
        => new() { IsHandled = false, Exception = ex };
}