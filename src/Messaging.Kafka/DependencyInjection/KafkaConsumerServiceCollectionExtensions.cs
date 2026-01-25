using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Messaging.Kafka.Consumer;
using Messaging.Kafka.Dispatching;
using Messaging.Kafka.Factories;
using Messaging.Kafka.Registry;
using Messaging.Kafka.Topics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.DependencyInjection;

public static class KafkaConsumerServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaConsumerMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<KafkaConsumerOptions>()
            .Bind(configuration.GetSection("Kafka:Consumer"))
            .ValidateOnStart();
        
        services.AddSingleton<IValidateOptions<KafkaConsumerOptions>, KafkaConsumerOptionsValidator>();
        
        services.AddSingleton<IKafkaConsumerTopicResolver, KafkaConsumerTopicResolver>();
        services.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
        services.AddSingleton<IKafkaHandlerRegistry, KafkaHandlerRegistry>();
        services.AddSingleton<IKafkaMessageDispatcher, KafkaMessageDispatcher>();
        services.AddSingleton<IKafkaMessagePump, KafkaMessagePump>();

        return services;
    }
}