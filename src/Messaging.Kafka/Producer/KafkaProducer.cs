using System.Diagnostics;
using Confluent.Kafka;
using Messaging.Abstractions;
using Messaging.Kafka.Producer.Factories;
using Microsoft.Extensions.Logging;
using Polly;

namespace Messaging.Kafka.Producer;

public sealed class KafkaProducer : IKafkaProducer, IAsyncDisposable
{
    private static readonly ActivitySource ActivitySource =
        new("messaging.kafka.producer");

    private readonly ILogger<KafkaProducer> _logger;

    private readonly IProducer<string, string> _producer;
    private readonly IKafkaMessageSerializer _serializer;
    private readonly IKafkaProducerTopicResolver _kafkaProducerTopicResolver;
    private readonly IAsyncPolicy _retryPolicy;

    public KafkaProducer(
        ILogger<KafkaProducer> logger,
        IKafkaProducerFactory producerFactory,
        IKafkaRetryPolicyFactory retryPolicyFactory,
        IKafkaMessageSerializer serializer,
        IKafkaProducerTopicResolver kafkaProducerTopicResolver)
    {
        _logger = logger;
        _serializer = serializer;
        _kafkaProducerTopicResolver = kafkaProducerTopicResolver;

        _producer = producerFactory.Create();
        _retryPolicy = retryPolicyFactory.Create();
    }

    public async Task ProduceAsync<T>(
        string key,
        T message,
        CancellationToken ct = default)
    {
        var eventType = typeof(T).Name;
        var topic = _kafkaProducerTopicResolver.Resolve<T>();

        using var activity = ActivitySource.StartActivity(
            "kafka.produce",
            ActivityKind.Producer);

        activity?.SetTag("messaging.system", "kafka");
        activity?.SetTag("messaging.destination", topic);
        activity?.SetTag("messaging.message_type", eventType);

        _logger.LogInformation(
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
                        Value = _serializer.Serialize(message)
                    },
                    token);

                _logger.LogInformation(
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

                _logger.LogCritical(
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

                _logger.LogWarning(
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
        _logger.LogInformation("Shutting down Kafka producer");

        try
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Kafka producer flush failed");
        }
        finally
        {
            _producer.Dispose();
        }

        await Task.CompletedTask;
    }
}