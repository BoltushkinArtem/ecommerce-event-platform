using Messaging.Abstractions.Handlers;
using OrderService.Contracts.Events;

namespace OrderService.Worker.Handlers;

public sealed class OrderCreatedHandler(ILogger<OrderCreatedHandler> logger) : IMessageHandler<OrderCreated>
{
    public async Task HandleAsync(
        OrderCreated message,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Start handling OrderCreated event. OrderId={OrderId}, CustomerId={CustomerId}, TotalAmount={TotalAmount}",
            message.OrderId,
            message.CustomerId,
            message.TotalAmount);

        logger.LogInformation(
            "OrderCreated event successfully handled. OrderId={OrderId}",
            message.OrderId);

        await Task.CompletedTask;
    }
}