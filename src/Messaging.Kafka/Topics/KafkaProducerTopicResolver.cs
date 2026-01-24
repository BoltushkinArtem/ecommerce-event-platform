using Messaging.Kafka.Configuration;
using Messaging.Kafka.Topics.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Topics;

public sealed class KafkaProducerTopicResolver(
    IOptions<KafkaProducerOptions> options,
    ILogger<KafkaTopicResolver<KafkaProducerOptions>> logger)
    : KafkaTopicResolver<KafkaProducerOptions>(options, logger), IKafkaProducerTopicResolver;