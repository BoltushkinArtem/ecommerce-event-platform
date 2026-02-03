using Messaging.Abstractions.Runtime;
using Messaging.Kafka.Consumer.Factories;
using Messaging.Kafka.Consumer.Pipeline;
using Messaging.Kafka.Consumer.Pipeline.Steps;
using Messaging.Kafka.Consumer.Pump;
using Messaging.Kafka.Consumer.Registry;
using Messaging.Kafka.Core.Configuration;
using Messaging.Kafka.Runtime;
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
        
        services.AddSingleton<IKafkaPipeline, KafkaPipeline>();

        services.AddSingleton<IKafkaPipelineStep, DeserializeStep>();
        services.AddSingleton<IKafkaPipelineStep, ValidateStep>();
        services.AddSingleton<IKafkaPipelineStep, HandleStep>();
        services.AddSingleton<IKafkaPipelineStep, RetryStep>();
        
        services.AddSingleton<IKafkaMessagePump, KafkaMessagePump>();
        services.AddSingleton<IMessageWorker, KafkaMessageWorker>();

        return services;
    }
}