using Messaging.Kafka.Consumer;
using Messaging.Kafka.Consumer.Dispatching;
using Messaging.Kafka.Consumer.Factories;
using Messaging.Kafka.Consumer.Pump;
using Messaging.Kafka.Consumer.Registry;
using Messaging.Kafka.Core.Configuration;
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
        services.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
        services.AddSingleton<IKafkaHandlerRegistry, KafkaHandlerRegistry>();
        services.AddSingleton<IKafkaMessageDispatcher, KafkaMessageDispatcher>();
        services.AddSingleton<IKafkaMessagePump, KafkaMessagePump>();

        return services;
    }
}