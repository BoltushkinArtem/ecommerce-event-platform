namespace OrderService.Contracts.Events;

public class OrderCreated
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    
    public OrderCreated() { }

    public OrderCreated(Guid orderId, Guid customerId, decimal totalAmount, DateTime createdAt)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        CreatedAt = createdAt;
    }
}