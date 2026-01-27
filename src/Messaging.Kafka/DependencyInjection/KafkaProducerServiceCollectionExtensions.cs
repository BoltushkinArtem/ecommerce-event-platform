
using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Messaging.Kafka.Factories;
using Messaging.Kafka.Producer;
using Messaging.Kafka.Topics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        
        services.AddSingleton<IValidateOptions<KafkaProducerOptions>, KafkaProducerOptionsValidator>();
        
        services.AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
        services.AddSingleton<IKafkaRetryPolicyFactory, KafkaRetryPolicyFactory>();
        services.AddSingleton<IMessagePublisher, MessagePublisher>();

        return services;
    }
}