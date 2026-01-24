using Confluent.Kafka;
using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Factories;

public sealed class KafkaProducerFactory
    : IKafkaProducerFactory
{
    private readonly KafkaOptions _kafkaOptions;
    private readonly KafkaProducerOptions _producerOptions;
    private readonly ILogger<KafkaProducerFactory> _logger;

    public KafkaProducerFactory(
        IOptions<KafkaOptions> kafkaOptions,
        IOptions<KafkaProducerOptions> producerOptions,
        ILogger<KafkaProducerFactory> logger)
    {
        _kafkaOptions = kafkaOptions.Value;
        _producerOptions = producerOptions.Value;
        _logger = logger;
    }

    public IProducer<string, string> Create()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaOptions.BootstrapServers,
            Acks = _producerOptions.Acks,
            EnableIdempotence = _producerOptions.EnableIdempotence
        };

        _logger.LogInformation(
            "Creating Kafka ProducerConfig. BootstrapServers={Servers}, Acks={Acks}, Idempotence={Idempotence}",
            _kafkaOptions.BootstrapServers,
            _producerOptions.Acks,
            _producerOptions.EnableIdempotence);

        return new ProducerBuilder<string, string>(config).Build();
    }
}