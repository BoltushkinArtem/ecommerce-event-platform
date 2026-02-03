using Messaging.Abstractions.Publishing;
using Messaging.Kafka.Core.Configuration;
using Messaging.Kafka.Producer.Factories;
using Messaging.Kafka.Producer.Publishing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.DependencyInjection;

public static class KafkaProducerServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaProducerMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<KafkaProducerOptions>()
            .Bind(configuration.GetSection("Kafka:Producer"))
            .ValidateOnStart();
        
        services.AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
        services.AddSingleton<IKafkaRetryPolicyFactory, KafkaRetryPolicyFactory>();
        services.AddSingleton<IMessagePublisher, MessagePublisher>();

        return services;
    }
}