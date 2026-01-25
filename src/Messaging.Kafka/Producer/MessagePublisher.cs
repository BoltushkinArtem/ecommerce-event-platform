using System.Diagnostics;
using Confluent.Kafka;
using Messaging.Abstractions;
using Messaging.Kafka.Factories;
using Messaging.Kafka.Serialization;
using Messaging.Kafka.Topics;
using Microsoft.Extensions.Logging;
using Polly;

namespace Messaging.Kafka.Producer;

public sealed class MessagePublisher(
    ILogger<MessagePublisher> logger,
    IKafkaProducerFactory producerFactory,
    IKafkaRetryPolicyFactory retryPolicyFactory,
    IKafkaMessageSerializer serializer,
    IKafkaProducerTopicResolver kafkaProducerTopicResolver)
    : IMessagePublisher, IAsyncDisposable
{
    private static readonly ActivitySource ActivitySource =
        new("messaging.kafka.producer");

    private readonly IProducer<string, string> _producer = producerFactory.Create();
    private readonly IAsyncPolicy _retryPolicy = retryPolicyFactory.Create();

    public async Task ProduceAsync<T>(
        string key,
        T message,
        CancellationToken ct = default)
    {
        var eventType = typeof(T).Name;
        var topic = kafkaProducerTopicResolver.Resolve<T>();

        using var activity = ActivitySource.StartActivity(
            "kafka.produce",
            ActivityKind.Producer);

        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", topic);
        activity?.SetTag("messaging.message_type", eventType);

        logger.LogInformation(
            "Producing Kafka event. EventType={EventType}, Topic={Topic}",
            eventType,
            topic);

        await _retryPolicy.ExecuteAsync(async token  =>
        {
            try
            {
                var result = await _producer.ProduceAsync(
                    topic,
                    new Message<string, string>
                    {
                        Key = key,
                        Value = serializer.Serialize(message)
                    },
                    token);

                logger.LogInformation(
                    "Kafka event produced successfully. " +
                    "EventType={EventType}, Topic={Topic}, Partition={Partition}, Offset={Offset}",
                    eventType,
                    result.Topic,
                    result.Partition.Value,
                    result.Offset.Value);
            }
            catch (ProduceException<string, string> ex) when (ex.Error.IsFatal)
            {
                activity?.SetStatus(
                    ActivityStatusCode.Error,
                    "Fatal Kafka error");

                logger.LogCritical(
                    ex,
                    "Fatal Kafka error. Producer is not recoverable. Topic={Topic}",
                    topic);
                throw;
            }
            catch (ProduceException<string, string> ex)
            {
                activity?.SetStatus(
                    ActivityStatusCode.Error,
                    "Transient Kafka error");

                logger.LogWarning(
                    ex,
                    "Transient Kafka produce failure. EventType={EventType}, Topic={Topic}",
                    eventType,
                    topic);

                throw;
            }
        }, ct);
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogInformation("Shutting down Kafka producer");

        try
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Kafka producer flush failed");
        }
        finally
        {
            _producer.Dispose();
        }

        await Task.CompletedTask;
    }
}