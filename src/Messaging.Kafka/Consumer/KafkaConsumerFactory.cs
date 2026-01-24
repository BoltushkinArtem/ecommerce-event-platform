using Confluent.Kafka;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Consumer;

public sealed class KafkaConsumerFactory : IKafkaConsumerFactory
{
    private readonly KafkaOptions _kafkaOptions;
    private readonly KafkaConsumerOptions _kafkaConsumerOptions;
    private readonly ILogger<KafkaConsumerFactory> _logger;

    public KafkaConsumerFactory(
        IOptions<KafkaOptions> kafkaOptions,
        IOptions<KafkaConsumerOptions> kafkaConsumerOptions,
        ILogger<KafkaConsumerFactory> logger)
    {
        _kafkaOptions = kafkaOptions.Value;
        _kafkaConsumerOptions = kafkaConsumerOptions.Value;
        _logger = logger;
    }

    public IConsumer<string, string> Create()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaConsumerOptions.GroupId,
            EnableAutoCommit = _kafkaConsumerOptions.EnableAutoCommit,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _logger.LogInformation(
            "Creating Kafka consumer. GroupId={GroupId}, BootstrapServers={BootstrapServers}",
            config.GroupId,
            config.BootstrapServers);

        return new ConsumerBuilder<string, string>(config)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
    }
}