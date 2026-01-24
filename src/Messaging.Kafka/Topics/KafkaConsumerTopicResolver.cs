using Messaging.Kafka.Configuration;
using Messaging.Kafka.Topics.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Topics;

public sealed class KafkaConsumerTopicResolver(
    IOptions<KafkaConsumerOptions> options,
    ILogger<KafkaTopicResolver<KafkaConsumerOptions>> logger)
    : KafkaTopicResolver<KafkaConsumerOptions>(options, logger),
        IKafkaConsumerTopicResolver;