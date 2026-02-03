namespace Messaging.Kafka.Consumer.Pipeline;

public sealed record KafkaProcessingResult(
    KafkaCommitDecision Decision,
    Exception? Error = null)
{
    public static KafkaProcessingResult Commit() =>
        new(KafkaCommitDecision.Commit);

    public static KafkaProcessingResult Skip(Exception? ex = null) =>
        new(KafkaCommitDecision.Skip, ex);

    public static KafkaProcessingResult Retry(Exception ex) =>
        new(KafkaCommitDecision.Retry, ex);
}