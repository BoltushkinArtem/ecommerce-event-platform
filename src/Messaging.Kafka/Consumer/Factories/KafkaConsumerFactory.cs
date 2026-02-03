using Confluent.Kafka;
using Messaging.Kafka.Core.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Consumer.Factories;

public sealed class KafkaConsumerFactory(
    IOptions<KafkaOptions> kafkaOptions,
    IOptions<KafkaConsumerOptions> kafkaConsumerOptions,
    ILogger<KafkaConsumerFactory> logger)
    : IKafkaConsumerFactory
{
    private readonly KafkaOptions _kafkaOptions = kafkaOptions.Value;
    private readonly KafkaConsumerOptions _kafkaConsumerOptions = kafkaConsumerOptions.Value;

    public IConsumer<string, string> Create()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            GroupId = _kafkaConsumerOptions.GroupId,
            EnableAutoCommit = _kafkaConsumerOptions.EnableAutoCommit,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        logger.LogInformation(
            "Creating Kafka consumer. GroupId={GroupId}, BootstrapServers={BootstrapServers}",
            config.GroupId,
            config.BootstrapServers);

        return new ConsumerBuilder<string, string>(config)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
    }
}