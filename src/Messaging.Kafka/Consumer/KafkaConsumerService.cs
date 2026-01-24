using System.Diagnostics;
using Confluent.Kafka;
using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Messaging.Kafka.Factories;
using Messaging.Kafka.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Consumer;

public sealed class KafkaConsumerService : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("messaging.kafka.consumer");

    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly IKafkaHandlerRegistry _registry;
    private readonly IServiceProvider _serviceProvider;
    private readonly KafkaConsumerOptions _options;
    private readonly IKafkaMessageSerializer _serializer;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IKafkaConsumerFactory consumerFactory,
        IKafkaHandlerRegistry registry,
        IServiceProvider serviceProvider,
        IOptions<KafkaConsumerOptions> options,
        IKafkaMessageSerializer serializer)
    {
        _logger = logger;
        _consumer = consumerFactory.Create();
        _registry = registry;
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _serializer = serializer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var topics = _registry.Topics;

        if (topics.Count <= 0)
        {
            _logger.LogWarning("No Topics found");
            return;
        }

        _consumer.Subscribe(topics);
        _logger.LogInformation(
            "Subscribed to topics: {Topics}",
            string.Join(", ", topics));

        var semaphore = new SemaphoreSlim(_options.MaxDegreeOfParallelism);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(TimeSpan.FromMilliseconds(500));
                if (result == null)
                    continue;

                await semaphore.WaitAsync(stoppingToken);

                _ = Task.Run(async () =>
                {
                    using var activity = ActivitySource.StartActivity(
                        "kafka.consume",
                        ActivityKind.Consumer);

                    activity?.SetTag("messaging.system", "kafka");
                    activity?.SetTag("messaging.topic", result.Topic);
                    activity?.SetTag("messaging.partition", result.Partition.Value);

                    try
                    {
                        var descriptor = _registry.GetDescriptor(result.Topic);
                        
                        var message = _serializer.Deserialize(result.Message.Value, descriptor.EventType);

                        if (message is null)
                            throw new InvalidOperationException(
                                $"Failed to deserialize message for topic '{result.Topic}'");

                        using var scope = _serviceProvider.CreateScope();

                        var handler = scope.ServiceProvider
                            .GetRequiredService(descriptor.HandlerType);

                        await ((dynamic)handler)
                            .HandleAsync((dynamic)message, stoppingToken);

                        _consumer.Commit(result);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Failed to process message from topic {Topic}",
                            result.Topic);

                        throw;
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, stoppingToken);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka consume error");
            }
        }
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
