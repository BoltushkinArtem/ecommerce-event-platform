namespace Messaging.Kafka.Consumer.Pipeline;

public enum KafkaCommitDecision
{
    Commit,
    Skip,
    Retry
}