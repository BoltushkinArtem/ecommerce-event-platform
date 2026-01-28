using Messaging.Abstractions;
using Messaging.Abstractions.Contracts;

namespace OrderService.Contracts.Events;

[EventContract("OrderCreated")]
public class OrderCreated(Guid orderId, Guid customerId, decimal totalAmount, DateTime createdAt)
{
    public Guid OrderId { get; init; } = orderId;
    public Guid CustomerId { get; init; } = customerId;
    public decimal TotalAmount { get; init; } = totalAmount;
    public DateTime CreatedAt { get; init; } = createdAt;
}