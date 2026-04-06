# Architecture Documentation

## Overview

This project implements an **event-driven microservices architecture** for an e-commerce platform. Services communicate through both **synchronous** (REST/gRPC) and **asynchronous** (RabbitMQ/Kafka) patterns.

## Service Architecture

### High-Level Design

```
┌─────────────────────────────────────────────────────────────────────────┐
│                           Angular Frontend                             │
│                        (Order/Inventory UI)                            │
└────────┬────────────────────────────────────┬──────────────────────────┘
         │ REST API                           │ REST API
         ▼                                    ▼
┌─────────────────────┐            ┌──────────────────────────────────────┐
│  PedidoService      │            │  EstoqueService                      │
│  (Order Management) │            │  (Inventory Control)                 │
│  ┌───────────────┐  │            │  ┌────────────────┐                  │
│  │ PostgreSQL    │  │            │  │ PostgreSQL     │                  │
│  │ MongoDB       │  │            │  └────────────────┘                  │
│  └───────────────┘  │            └──────────┬───────────────────────────┘
└──────────┬──────────┘                       │
           │                                  │
           │         ┌─────────────────────┐  │
           │         │   RabbitMQ / Kafka  │  │
           └────────►│    (Event Bus)      │◄─┘
                     │                     │
           ┌────────►│                     │◄────────┐
           │         └──────────┬──────────┘         │
           │                    │                    │
┌──────────▼───────────────┐    │    ┌───────────────▼────────────────┐
│  PagamentoService        │    │    │  NotificacaoService            │
│  (Payment Processing)    │    │    │  (Notifications & Audit)       │
│  ┌────────────────────┐  │    │    │  ┌──────────────────┐          │
│  │ PostgreSQL         │  │    │    │  │ PostgreSQL       │          │
│  └────────────────────┘  │    │    │  │ Redis            │          │
└──────────────────────────┘    │    │  └──────────────────┘          │
                                │    └────────────────────────────────┘
                       ┌────────▼────────┐
                       │  WorkerEmail    │
                       │  (Background)   │
                       └─────────────────┘
```

## Communication Patterns

### 1. Synchronous Communication

**REST APIs** - Used for:
- Frontend → Service communication
- Direct service-to-service queries (when needed)
- Health checks and monitoring

**gRPC** - Used for:
- Low-latency service-to-service calls
- Internal service communication
- High-throughput scenarios

### 2. Asynchronous Communication

**RabbitMQ** (Primary message broker):
- **Pattern:** Pub/Sub with topic exchanges
- **Use cases:** Task queues, event notifications, background jobs
- **Features:** Dead Letter Queue (DLQ), message acknowledgment, retry logic

**Kafka** (Event streaming platform):
- **Pattern:** Event log / streaming
- **Use cases:** Event sourcing, audit trails, analytics
- **Features:** Event replay, partitioning, consumer groups

## Event Flow

### Order Creation Flow

```
1. Client → POST /api/orders → PedidoService
2. PedidoService → saves to PostgreSQL
3. PedidoService → publishes "PedidoCriado" event → RabbitMQ
4. EstoqueService consumes "PedidoCriado":
   ├─ Reserves stock
   └─ publishes "EstoqueReservado" event
5. PagamentoService consumes "EstoqueReservado":
   ├─ Processes payment (mock)
   └─ publishes "PagamentoAprovado" or "PagamentoRecusado"
6. PedidoService consumes payment result:
   ├─ If approved: confirms order → publishes "PedidoConfirmado"
   └─ If refused: cancels order → publishes "PedidoFalhou"
7. NotificacaoService consumes ALL events:
   └─ Sends notifications to user
```

### Saga Pattern (Distributed Transaction)

This project implements the **Saga Pattern** using **choreography** (event-based coordination):

```
Normal Flow:
  PedidoCriado → EstoqueReservado → PagamentoAprovado → PedidoConfirmado

Compensation Flow (Payment Failed):
  PagamentoRecusado → EstoqueLiberaReserva → PedidoFalhou
```

**Key Principle:** Each service publishes events. Other services react to those events. There's no central coordinator.

## Database Architecture

### Polyglot Persistence

| Database | Used By | Purpose |
|----------|---------|---------|
| **PostgreSQL** | All services | Primary relational data (ACID compliance) |
| **MongoDB** | PedidoService | Order history, analytics (flexible schema) |
| **Redis** | NotificacaoService | Caching, rate limiting, session storage |

### Data Isolation

Each service owns its database. No service directly accesses another service's database.

## Resilience Patterns

### 1. Circuit Breaker (Polly)

Prevents cascading failures when a service is down:

```
Closed → (failures exceed threshold) → Open → (timeout) → Half-Open → (test succeeds) → Closed
```

### 2. Retry with Exponential Backoff

Handles transient failures:

```
Attempt 1 → wait 1s
Attempt 2 → wait 2s
Attempt 3 → wait 4s
Give up → send to DLQ
```

### 3. Dead Letter Queue (DLQ)

Messages that fail repeatedly are moved to a DLQ for manual inspection:

```
Main Queue → (3 retries failed) → DLX Exchange → DLQ
```

### 4. Idempotency

Prevents duplicate processing of the same message:

```json
{
  "eventId": "unique-guid",
  "eventType": "PedidoCriado",
  "data": { ... }
}
```

Services track processed event IDs to avoid duplicates.

## Message Broker Configuration

### RabbitMQ Setup

- **Exchange:** `ecommerce.events` (topic exchange)
- **Queues:** One per service + DLQ
- **Routing Keys:** `pedido.*`, `estoque.*`, `pagamento.*`, `email.*`

### Kafka Setup

- **Topics:** Orders, Payments, Inventory, Notifications
- **Consumer Groups:** One per service
- **Partitioning:** By order/customer ID

## Security Considerations

- All sensitive data in environment variables (not hardcoded)
- Database credentials managed via Docker secrets (production)
- HTTPS enabled for all REST/gRPC endpoints
- API rate limiting (future enhancement)

## Monitoring & Observability

- **Health Checks:** `/health` endpoint on each service
- **Structured Logging:** Serilog with console and file sinks
- **Metrics:** Prometheus format (future enhancement)
- **Tracing:** Distributed tracing (future enhancement)

## Future Enhancements

- [ ] API Gateway (YARP or Ocelot)
- [ ] Distributed tracing (OpenTelemetry)
- [ ] Kubernetes deployment manifests
- [ ] CI/CD pipelines (GitHub Actions)
- [ ] Load testing and performance optimization
- [ ] Real email notifications (SendGrid/AWS SES)
