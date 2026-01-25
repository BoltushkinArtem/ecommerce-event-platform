using Confluent.Kafka;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Factories;

public sealed class KafkaProducerFactory(
    IOptions<KafkaOptions> kafkaOptions,
    IOptions<KafkaProducerOptions> producerOptions,
    ILogger<KafkaProducerFactory> logger)
    : IKafkaProducerFactory
{
    private readonly KafkaOptions _kafkaOptions = kafkaOptions.Value;
    private readonly KafkaProducerOptions _producerOptions = producerOptions.Value;

    public IProducer<string, string> Create()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            Acks = _producerOptions.Acks,
            EnableIdempotence = _producerOptions.EnableIdempotence
        };

        logger.LogInformation(
            "Creating Kafka ProducerConfig. BootstrapServers={Servers}, Acks={Acks}, Idempotence={Idempotence}",
            _kafkaOptions.BootstrapServers,
            _producerOptions.Acks,
            _producerOptions.EnableIdempotence);

        return new ProducerBuilder<string, string>(config).Build();
    }
}