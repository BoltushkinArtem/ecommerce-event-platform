using Messaging.Abstractions.Publishing;
using Microsoft.AspNetCore.Http.HttpResults;
using OrderService.Contracts.Events;
using OrderService.Service.Contracts;

namespace OrderService.Service.Endpoints;

public static class CreateOrderEndpoint
{
    public static RouteHandlerBuilder MapCreateOrder(
        this IEndpointRouteBuilder app)
    {
        return app.MapPost("/orders",
            async Task<Results<Created, BadRequest<string>>> (
                CreateOrderRequest request,
                IMessagePublisher publisher,
                ILoggerFactory loggerFactory,
                CancellationToken ct) =>
            {
                var logger = loggerFactory.CreateLogger("OrderService.Service.Endpoints.CreateOrderEndpoint");

                logger.LogInformation(
                    "Create order request received. CustomerId={CustomerId}, TotalAmount={TotalAmount}",
                    request.CustomerId,
                    request.TotalAmount);

                if (request.TotalAmount <= 0)
                {
                    logger.LogWarning(
                        "Invalid order request: TotalAmount must be positive. Value={TotalAmount}",
                        request.TotalAmount);

                    return TypedResults.BadRequest("TotalAmount must be positive");
                }

                if (request.CustomerId == Guid.Empty)
                {
                    logger.LogWarning("Invalid order request: CustomerId is empty");
                    return TypedResults.BadRequest("CustomerId is required");
                }

                var orderId = Guid.NewGuid();

                var evt = new OrderCreated(
                    orderId,
                    request.CustomerId,
                    request.TotalAmount,
                    DateTime.UtcNow);

                logger.LogInformation(
                    "Publishing OrderCreated event. OrderId={OrderId}",
                    orderId);

                await publisher.ProduceAsync(
                    key: orderId.ToString(),
                    message: evt,
                    ct);

                logger.LogInformation(
                    "OrderCreated event published successfully. OrderId={OrderId}",
                    orderId);

                return TypedResults.Created($"/orders/{orderId}");
            });
    }
}