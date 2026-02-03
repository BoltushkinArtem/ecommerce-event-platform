# E-Commerce Event Platform — Lightweight Kafka Project

**A simple and lightweight .NET project for working with Apache Kafka**, designed for developers who value **clarity, control, and minimal complexity**.  

> Forget heavy, opinionated frameworks. This is Kafka **without the magic**.

---

## Why Use It?

* ✅ **Lightweight and modular** — use only what you need  
* ✅ **Transparent pipelines** — easy to debug and extend  
* ✅ **Domain-first design** — Kafka stays infrastructure, not a framework  
* ✅ **Production-ready building blocks** — retries, idempotency, topic management  
* ✅ **Easy customization** — swap serializers, handlers, or topics in seconds  

> Compared to MassTransit: fewer abstractions, less hidden logic, and full control over your messaging.

---

## Core Concepts

* **Messaging.Abstractions** — interfaces for publishing messages, handlers, and workers  
* **Messaging.Kafka** — Kafka implementation with pipelines, retry policies, and topic management  
* **Messaging.Runtime.Hosting** — background worker hosting for clean integration  

---

## Project Structure

```text
ecommerce-event-platform/
├── docker/
│   └── docker-compose.yml
├── src/
│   ├── Messaging.Abstractions/        # Core interfaces & contracts
│   ├── Messaging.Kafka/               # Kafka implementation
│   ├── Messaging.Runtime.Hosting/     # Worker hosting
│   ├── OrderService.Contracts/        # Domain events
│   ├── OrderService.Service/          # API & business logic
│   └── OrderService.Worker/           # Background workers & handlers
├── README.md
└── LICENSE
````

---

## Contract Setup (Domain Events)

Each domain event **must have the `EventKey` attribute** that matches the `EventKey` specified in your `appsettings.json`. This ensures that messages are correctly mapped to Kafka topics.

```csharp
using Messaging.Abstractions.Messages;

[EventKey("OrderCreated")]
public class OrderCreated
{
    public OrderCreated(Guid orderId, Guid customerId, decimal totalAmount, DateTime createdAt)
    {
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        CreatedAt = createdAt;
    }

    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
}
```

**Key points:**

* The string in `[EventKey("…")]` **must match the `EventKey` in appsettings.json** under the `Topics` section.
* This attribute allows the Kafka infrastructure to route the event to the correct topic automatically.

---

## Quick Start

### Start Kafka

```bash
docker compose up -d
```

---

## Producer (Publisher) Setup

### Configuration

``` json
"Kafka": {
  "BootstrapServers": "localhost:9094",
  "Producer": {
    "Acks": "All",
    "EnableIdempotence": true
  },
  "Topics": [
    {
      "EventKey": "OrderCreated",
      "Name": "orders.created"
    }
  ],
  "Retry": {
    "RetryCount": 3,
    "BaseDelayMs": 200
  }
}
```

**Parameter explanation:**

* **BootstrapServers** — Kafka broker address(es)
* **Producer.Acks** — message acknowledgment strategy (`All` = wait for all in-sync replicas)
* **Producer.EnableIdempotence** — ensures messages are not duplicated on retries
* **Topics** — map event keys to Kafka topic names
* **Retry.RetryCount** — maximum number of retries on failure
* **Retry.BaseDelayMs** — base delay between retries in milliseconds

### DI Setup

```csharp
builder.Services.AddKafkaCoreMessaging(builder.Configuration);
builder.Services.AddKafkaProducerMessaging(builder.Configuration);
```

### Sending Messages

```csharp
var evt = new OrderCreated(
    orderId,
    request.CustomerId,
    request.TotalAmount,
    DateTime.UtcNow);

await publisher.ProduceAsync(
    key: orderId.ToString(),
    message: evt,
    cancellationToken: ct);
```

---

## Consumer (Subscriber) Setup

### Configuration

``` json
"Kafka": {
  "BootstrapServers": "localhost:9094",
  "Consumer": {
    "GroupId": "order-service-group",
    "EnableAutoCommit": false
  },
  "Topics": [
    {
      "EventKey": "OrderCreated",
      "Name": "orders.created"
    }
  ],
  "Retry": {
    "RetryCount": 5,
    "BaseDelayMs": 100
  }
}
```

**Parameter explanation:**

* **BootstrapServers** — Kafka broker address(es)
* **Consumer.GroupId** — consumer group ID
* **Consumer.EnableAutoCommit** — if `false`, offsets are committed manually after successful handling
* **Topics** — map event keys to Kafka topic names
* **Retry.RetryCount** — max retry attempts for handling failure
* **Retry.BaseDelayMs** — base delay between retries in milliseconds

### Handler Example

```csharp
public sealed class OrderCreatedHandler : IMessageHandler<OrderCreated>
{
    private readonly ILogger<OrderCreatedHandler> _logger;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger)
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

        _logger.LogInformation(
            "OrderCreated event successfully handled. OrderId={OrderId}",
            message.OrderId);

        await Task.CompletedTask;
    }
}
```

### DI Setup

```csharp
// Core Kafka services
builder.Services.AddKafkaCoreMessaging(builder.Configuration);

// Consumer services
builder.Services.AddKafkaConsumerMessaging(builder.Configuration);

// Register handler
builder.Services.AddKafkaHandler<OrderCreated, OrderCreatedHandler>();

// Register background worker
builder.Services.AddMessageWorkerHostedRuntime();
```

---

## Design Philosophy

* **Simple > Complex**
* **Transparent > Hidden Magic**
* **Flexible > Opinionated**

This project is for **developers who want to work with Kafka directly, without extra framework overhead**.

---

## License

MIT License