using Messaging.Abstractions;
using Messaging.Kafka.Configuration;
using Messaging.Kafka.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.DependencyInjection;

public static class KafkaHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaHandler<TEvent, THandler>(
        this IServiceCollection services)
        where TEvent : class
        where THandler : class, IMessageHandler<TEvent>
    {
        services.AddScoped<IMessageHandler<TEvent>, THandler>();

        // using var sp = services.BuildServiceProvider();
        // var resolver = sp.GetRequiredService<IKafkaTopicResolver>();
        //
        // var topic = resolver.Resolve<TEvent>();
        
        services.Configure<KafkaHandlerRegistryOptions>(options =>
        {
            options.Handlers.Add(
                new KafkaHandlerOptions(
                    EventType: typeof(TEvent),
                    HandlerType: typeof(THandler)));
        });
        
        services.AddScoped<THandler>();

        return services;
    }
}