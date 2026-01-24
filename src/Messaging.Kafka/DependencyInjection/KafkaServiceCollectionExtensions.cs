using Messaging.Kafka.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.DependencyInjection;

public static class KafkaServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        KafkaMessagingMode mode)
    {
        services.AddKafkaCoreMessaging(configuration);
        
        if (mode is KafkaMessagingMode.Producer or KafkaMessagingMode.ProducerAndConsumer)
        {
            services.AddKafkaProducerMessaging(configuration);
        }
        if (mode is KafkaMessagingMode.Consumer or KafkaMessagingMode.ProducerAndConsumer)
        {
            services.AddKafkaConsumerMessaging(configuration);
        }

        return services;
    }
}