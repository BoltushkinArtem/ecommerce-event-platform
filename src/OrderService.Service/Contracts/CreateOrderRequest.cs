namespace OrderService.Service.Contracts;

public sealed record CreateOrderRequest
{
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
}