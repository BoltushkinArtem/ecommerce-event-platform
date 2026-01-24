using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        KafkaMessagingMode mode)
    {
        services.AddKafkaCoreMessaging(configuration);
        
        if (mode is KafkaMessagingMode.OnlyProducer or KafkaMessagingMode.ProducerAndConsumer)
        {
            services.AddKafkaProducerMessaging(configuration);
        }
        if (mode is KafkaMessagingMode.OnlyConsumer or KafkaMessagingMode.ProducerAndConsumer)
        {
            services.AddKafkaConsumerMessaging(configuration);
        }

        return services;
    }
}