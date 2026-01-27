using System.Linq.Expressions;
using Messaging.Abstractions;
using Messaging.Kafka.Serialization;
using Messaging.Kafka.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Registry;

public sealed class KafkaHandlerRegistry : IKafkaHandlerRegistry
{
    private readonly Dictionary<string, KafkaHandlerDescriptor> _byTopic;

    public KafkaHandlerRegistry(
        IOptions<KafkaHandlerRegistryOptions> options,
        IKafkaTopicResolver topicResolver,
        IKafkaMessageSerializer serializer)
    {
        _byTopic = new Dictionary<string, KafkaHandlerDescriptor>(
            options.Value.Handlers.Count);

        foreach (var h in options.Value.Handlers)
        {
            var topic = topicResolver.Resolve(h.EventType);

            var descriptor = BuildDescriptor(
                topic,
                h.EventType,
                h.HandlerType,
                serializer);

            _byTopic.Add(topic, descriptor);
        }
    }

    public KafkaHandlerDescriptor GetDescriptor(string topic)
        => _byTopic[topic];

    public IReadOnlyCollection<string> Topics => _byTopic.Keys;
    
    private static KafkaHandlerDescriptor BuildDescriptor(
        string topic,
        Type eventType,
        Type handlerType,
        IKafkaMessageSerializer serializer)
    {
        // parameters
        var spParam = Expression.Parameter(typeof(IServiceProvider), "sp");
        var payloadParam = Expression.Parameter(typeof(string), "payload");
        var ctParam = Expression.Parameter(typeof(CancellationToken), "ct");

        // serializer.Deserialize(payload, eventType)
        var deserializeCall = Expression.Call(
            Expression.Constant(serializer),
            nameof(IKafkaMessageSerializer.Deserialize),
            Type.EmptyTypes,
            payloadParam,
            Expression.Constant(eventType));

        // sp.GetRequiredService<THandler>()
        var getHandlerCall = Expression.Call(
            typeof(ServiceProviderServiceExtensions),
            nameof(ServiceProviderServiceExtensions.GetRequiredService),
            new[] { handlerType },
            spParam);

        // handler.HandleAsync((TEvent)message, ct)
        var handleMethod = handlerType.GetMethod(
            nameof(IMessageHandler<object>.HandleAsync),
            new[] { eventType, typeof(CancellationToken) })!;

        var handleCall = Expression.Call(
            getHandlerCall,
            handleMethod,
            Expression.Convert(deserializeCall, eventType),
            ctParam);

        var lambda = Expression.Lambda<
            Func<IServiceProvider, string, CancellationToken, Task>>(
            handleCall,
            spParam,
            payloadParam,
            ctParam);

        return new KafkaHandlerDescriptor(
            topic,
            lambda.Compile());
    }
}