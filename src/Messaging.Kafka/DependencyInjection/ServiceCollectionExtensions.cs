
using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Messaging.Kafka.Producer;
using Messaging.Kafka.Producer.Factories;
using Messaging.Kafka.Serialization;
using Messaging.Kafka.Topics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection("Kafka"))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<KafkaOptions>, KafkaOptionsValidator>();

        services.AddSingleton<IKafkaMessageSerializer, KafkaMessageSerializer>();
        services.AddSingleton<KafkaTopicResolver>();
        services.AddSingleton<IKafkaTopicResolver, KafkaTopicResolver>();

        services.AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
        services.AddSingleton<IKafkaRetryPolicyFactory, KafkaRetryPolicyFactory>();

        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        return services;
    }
}