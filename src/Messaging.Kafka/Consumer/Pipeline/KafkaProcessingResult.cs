namespace Messaging.Kafka.Consumer.Pipeline;

public sealed record KafkaProcessingResult(
    bool Success,
    Exception? Error = null)
{
    public static KafkaProcessingResult Ok() => new(true);
    public static KafkaProcessingResult Fail(Exception ex) => new(false, ex);
}