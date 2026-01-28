using Messaging.Abstractions;
using Messaging.Abstractions.Handlers;
using Messaging.Kafka.Consumer.Registry;
using Messaging.Kafka.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.DependencyInjection;

public static class KafkaHandlerServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaHandler<TEvent, THandler>(
        this IServiceCollection services)
        where TEvent : class
        where THandler : class, IMessageHandler<TEvent>
    {
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