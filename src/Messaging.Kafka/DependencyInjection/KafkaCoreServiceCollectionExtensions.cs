using Messaging.Kafka.Core.Attributes;
using Messaging.Kafka.Core.Configuration;
using Messaging.Kafka.Core.Serialization;
using Messaging.Kafka.Core.Topics;
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
        services.AddSingleton<IEventKeyResolver, EventKeyResolver>();
        services.AddSingleton<IKafkaTopicResolver, KafkaTopicResolver>();

        return services;
    }
}