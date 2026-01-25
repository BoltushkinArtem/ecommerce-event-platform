using Messaging.Abstractions;
using Messaging.Kafka.Registry;
using Messaging.Kafka.Topics;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.DependencyInjection;

public static class KafkaHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaHandler<TEvent, THandler>(
        this IServiceCollection services)
        where TEvent : class
        where THandler : class, IKafkaMessageHandler<TEvent>
    {
        services.AddScoped<IKafkaMessageHandler<TEvent>, THandler>();

        using var sp = services.BuildServiceProvider();
        var resolver = sp.GetRequiredService<IKafkaConsumerTopicResolver>();

        var topic = resolver.Resolve<TEvent>();

        services.AddSingleton(new KafkaHandlerDescriptor(
            Topic: topic,
            EventType: typeof(TEvent),
            HandlerType: typeof(THandler)));
        
        services.AddScoped<THandler>();

        return services;
    }
}