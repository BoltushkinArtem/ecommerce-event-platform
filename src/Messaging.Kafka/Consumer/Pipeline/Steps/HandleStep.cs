using Messaging.Kafka.Consumer.Factories;
using Messaging.Kafka.Consumer.Registry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer.Pipeline.Steps;

public sealed class HandleStep(
    IServiceScopeFactory scopeFactory,
    IKafkaHandlerRegistry registry,
    IHandlerInvokerFactory invokerFactory,
    ILogger<HandleStep> logger)
    : IKafkaPipelineStep
{
    public async Task<KafkaConsumeContext> ExecuteAsync(
        KafkaConsumeContext context,
        CancellationToken ct)
    {
        var descriptor = registry.GetDescriptor(
            context.ConsumeResult.Topic);
        
        var invoker = invokerFactory.GetOrCreate(
            descriptor.EventType,
            descriptor.HandlerType);

        // var messageContext = new MessageContext
        // {
        //     Topic = context.ConsumeResult.Topic,
        //     Key = context.ConsumeResult.Message.Key,
        //     Offset = context.ConsumeResult.Offset.Value,
        //     Headers = context.ConsumeResult.Message.Headers
        //         .ToDictionary(
        //             h => h.Key,
        //             h => System.Text.Encoding.UTF8.GetString(h.GetValueBytes()))
        // };

        logger.LogInformation(
            "Invoking handler {Handler} for event {Event}",
            descriptor.HandlerType.Name,
            descriptor.EventType.Name);
        

        using var scope = scopeFactory.CreateScope();
        
        await invoker(
            scope.ServiceProvider,
            context.Message!,
            ct);

        return await Task.FromResult(
            context with
            {
                IsHandled = true
            });
    }
}