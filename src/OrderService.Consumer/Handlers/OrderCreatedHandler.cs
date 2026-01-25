using Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using OrderService.Contracts.Events;

namespace OrderService.Consumer.Handlers;

public sealed class OrderCreatedHandler: IKafkaMessageHandler<OrderCreated>
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    // сюда позже добавятся:
    // private readonly IOrderRepository _orders;
    // private readonly IUnitOfWork _uow;
    // private readonly IKafkaProducer _producer;

    public OrderCreatedHandler(
        ILogger<OrderCreatedHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleAsync(
        OrderCreated message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Start handling OrderCreated event. OrderId={OrderId}, CustomerId={CustomerId}, TotalAmount={TotalAmount}",
            message.OrderId,
            message.CustomerId,
            message.TotalAmount);

        // 🧠 1. Идемпотентность (важно для Kafka!)
        // if (await _orders.ExistsAsync(message.OrderId, cancellationToken))
        // {
        //     _logger.LogWarning("Order {OrderId} already processed", message.OrderId);
        //     return;
        // }

        // 🧠 2. Бизнес-логика
        // var order = Order.Create(
        //     message.OrderId,
        //     message.CustomerId,
        //     message.Amount,
        //     message.CreatedAt);

        // await _orders.AddAsync(order, cancellationToken);
        // await _uow.SaveChangesAsync(cancellationToken);

        // 🧠 3. Возможная публикация нового события
        // await _producer.PublishAsync(
        //     new OrderValidated(order.Id),
        //     cancellationToken);

        _logger.LogInformation(
            "OrderCreated event successfully handled. OrderId={OrderId}",
            message.OrderId);

        await Task.CompletedTask;
    }
}