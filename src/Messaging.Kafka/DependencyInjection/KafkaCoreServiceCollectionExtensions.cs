using Messaging.Kafka.Configuration;
using Messaging.Kafka.Serialization;
using Messaging.Kafka.Topics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.DependencyInjection;

public static class KafkaCoreServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaCoreMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<KafkaOptions>()
            .Bind(configuration.GetSection("Kafka"))
            .ValidateOnStart();
        
        services.AddSingleton<IValidateOptions<KafkaOptions>, KafkaOptionsValidator>();
        services.AddSingleton<IKafkaMessageSerializer, KafkaMessageSerializer>();
        services.AddSingleton<IKafkaTopicResolver, KafkaTopicResolver>();

        return services;
    }
}