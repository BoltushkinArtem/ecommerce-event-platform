# E-Commerce Event Platform

A production-grade, event-driven e-commerce platform built with **.NET** and **Apache Kafka**,
designed to demonstrate reliable messaging, clean architecture, and scalable integration patterns.

This repository focuses on **Kafka-based messaging infrastructure**, implemented as reusable building blocks and integrated into a real business domain.

---

## Overview

The **E-Commerce Event Platform** is an event-driven system where core business domains
(orders, payments, delivery, analytics) communicate exclusively through Kafka events.

The project emphasizes:

* clean separation of **business logic** and **messaging infrastructure**
* production-ready Kafka configuration
* explicit abstractions and extension points
* maintainability and scalability

---

## Architecture Principles

* **Event-Driven Architecture**
* **Infrastructure as Building Blocks**
* **Explicit contracts between services**
* **No framework leakage into domain logic**
* **Configuration-driven behavior**

Kafka is treated as **infrastructure**, not as application code.

---

## Project Structure

```text
ecommerce-event-platform/
├── docker/
│   └── docker-compose.yml
│
├── README.md
│
└── src/
    ├── BuildingBlocks/
    │   └── Messaging/
    │       ├── Abstractions/
    │       │   └── Messaging.Abstractions/
    │       │       ├── IKafkaProducer.cs
    │       │       ├── IKafkaMessageSerializer.cs
    │       │       └── IKafkaTopicResolver.cs
    │       │
    │       └── Kafka/
    │           ├── Configuration/
    │           │   ├── KafkaOptions.cs
    │           │   ├── KafkaOptionsValidator.cs
    │           │   ├── KafkaProducerOptions.cs
    │           │   ├── KafkaRetryOptions.
    │           │   └── KafkaTopicOptions.cs
    │           │
    │           ├── Producer/
    │           │   ├── Factories/
    │           │   │   ├── IKafkaProducerFactory.cs
    │           │   │   ├── IKafkaRetryPolicyFactory.cs
    │           │   │   ├── KafkaProducerFactory.cs
    │           │   │   └── KafkaRetryPolicyFactory.cs
    │           │   │
    │           │   └── KafkaProducer.cs
    │           │
    │           ├── Serialization/
    │           │   └── KafkaMessageSerializer.cs
    │           │
    │           ├── Topics/
    │           │   ├── KafkaTopicDefinition.cs
    │           │   └── KafkaTopicResolver.cs
    │           │
    │           └── DependencyInjection/
    │               └── ServiceCollectionExtensions.cs
    │
    └── OrderService/
        ├── OrderService.Contracts/
        │   └── Events/
        │       └── OrderCreated.cs
        │
        └── OrderService.Producer.Console/
            ├── appsettings.json
            └── Program.cs
```

---

## Messaging Abstractions

The `Messaging.Abstractions` project defines **infrastructure-agnostic contracts**.

### IKafkaProducer

```csharp
public interface IKafkaProducer
{
    Task ProduceAsync<T>(
        T message,
        string key,
        CancellationToken cancellationToken = default);
}
```

Purpose:

* isolates application code from Kafka APIs
* enables testing and future extensibility
* allows multiple producer implementations

---

### IKafkaMessageSerializer

Responsible for message serialization.

* decouples message format from Kafka
* enables future migration to Avro / Protobuf
* enforces a single serialization strategy

---

### IKafkaTopicResolver

Maps domain events to Kafka topics.

* event ≠ topic
* centralized routing logic
* supports versioning and naming conventions

---

## Kafka Messaging Implementation

The `Messaging.Kafka` project provides a **production-ready Kafka implementation** of the abstractions.

### Configuration

Strongly-typed and validated configuration:

* `KafkaOptions`
* `KafkaProducerOptions`
* `KafkaRetryOptions`
* `KafkaTopicOptions`

All Kafka behavior is **configuration-driven**.

---

### Producer

`KafkaProducer` implements `IKafkaProducer` and encapsulates:

* producer lifecycle management
* retry handling
* acknowledgment configuration
* delivery guarantees

Producer creation and retry behavior are managed via dedicated factories.

---

### Serialization

`KafkaMessageSerializer` currently uses JSON serialization.

The design intentionally allows seamless migration to:

* Schema Registry
* Avro / Protobuf
* versioned schemas

---

### Topic Management

* `KafkaTopicDefinition`
* `KafkaTopicResolver`

Topics are treated as **explicit infrastructure resources**, not inline strings.

---

### Dependency Injection

Kafka messaging is integrated through a single extension:

```csharp
services.AddKafkaMessaging(configuration);
```

No Kafka-specific code leaks into application services.

---

## Order Service

### Domain Event

```csharp
public record OrderCreated(
    Guid OrderId,
    DateTime CreatedAt,
    decimal TotalAmount);
```

The event is defined in a dedicated **Contracts** project and represents a stable integration boundary.

---

### Producer Application

`OrderService.Producer.Console` demonstrates how a service publishes domain events
without direct dependency on Kafka infrastructure.

---

## Configuration

The platform is fully configuration-driven. Kafka and logging behavior are defined via `appsettings.json`.

### Logging

Logging is configured using **Serilog** and outputs structured logs to the console.

Key aspects:

* Centralized minimum log level configuration
* Reduced noise from framework logs (`Microsoft`, `System`)
* Enrichment with contextual metadata (thread id, machine name)

Example:

``` json
"Serilog": {
  "Using": [ "Serilog.Sinks.Console" ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "Enrich": [ "FromLogContext", "WithThreadId", "WithMachineName" ]
}
```

---

### Kafka Configuration

Kafka configuration is defined under the `Kafka` section and mapped to strongly-typed options.

#### Bootstrap Servers

``` json
"BootstrapServers": "localhost:9094"
```

Specifies the Kafka cluster entry point. Multiple brokers can be provided for production environments.

---

#### Producer Settings

``` json
"Producer": {
  "Acks": "All",
  "EnableIdempotence": true
}
```

* `Acks = All` ensures messages are acknowledged by all in-sync replicas
* `EnableIdempotence = true` guarantees safe retries without duplicate message delivery

---

#### Topic Mapping

``` json
"Topics": [
  {
    "Event": "OrderCreated",
    "Name": "orders.created"
  }
]
```

Defines an explicit mapping between domain events and Kafka topics.

This approach:

* avoids hard-coded topic names
* centralizes routing logic
* simplifies event versioning

---

#### Retry Policy

``` json
"Retry": {
  "RetryCount": 5,
  "BaseDelayMs": 100
}
```

Controls producer retry behavior:

* `RetryCount` — maximum number of retry attempts
* `BaseDelayMs` — base delay used for retry backoff

Retry behavior is applied consistently via a dedicated retry policy factory.

---

## Running the Platform

### Start Kafka Infrastructure

```bash
docker compose up -d
```

### Run the Producer

```bash
dotnet run --project src/OrderService/OrderService.Producer
```

Events can be inspected using Kafka UI.

---

## Design Decisions

* Kafka clients are **long-lived** and centrally managed
* All Kafka configuration is externalized
* Messaging concerns are isolated from domain logic
* The system is designed to evolve toward:

    * transactional producers
    * exactly-once semantics
    * stream processing
    * observability and monitoring

---

## Roadmap

* Kafka Consumers with manual offset management
* Background processing and resilience patterns
* Retry topics and dead-letter queues
* Transactional messaging
* Stream processing and aggregations
* Observability and metrics

---

## Status

Active development
The project evolves incrementally with a strong focus on correctness, reliability, and architectural clarity.

---

## License

MIT License
