# Architecture Documentation

## Overview

This project implements an **event-driven microservices architecture** for an e-commerce platform. Services communicate through **Kafka as the event backbone** for business events and **RabbitMQ for task queues** (notifications, background jobs).

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
           │         │       Kafka         │  │
           └────────►│   (Event Backbone)  │◄─┘
                     │                     │
           ┌────────►│                     │◄────────┐
           │         └──────────┬──────────┘         │
           │                    │                    │
┌──────────▼───────────────┐    │    ┌───────────────▼────────────────┐
│  PagamentoService        │    │    │  NotificacaoService            │
│  (Payment Processing)    │    │    │  (Reads Kafka → Tasks)         │
│  ┌────────────────────┐  │    │    │  ┌──────────────────┐          │
│  │ PostgreSQL         │  │    │    │  │ PostgreSQL       │          │
│  └────────────────────┘  │    │    │  │ Redis            │          │
└──────────────────────────┘    │    │  └──────────────────┘          │
                                │    └──────────┬─────────────────────┘
                       ┌────────▼────────┐       │
                       │  WorkerEmail    │◄──────┘
                       │  (RabbitMQ)     │  RabbitMQ tasks
                       └─────────────────┘
```

## Communication Patterns

### 1. Kafka — Event Backbone (Business Events)

**Pattern:** Event sourcing / event streaming

**Topics:**
| Topic | Partitions | Retention | Purpose |
|-------|------------|-----------|---------|
| `orders` | 3 | 7 days | Order lifecycle events |
| `payments` | 3 | 7 days | Payment processing events |
| `inventory` | 2 | 7 days | Stock reservation/release events |
| `notifications` | 2 | 3 days | Notification dispatch events |

**Used for:**
- Order lifecycle events (`PedidoCriado`, `PedidoConfirmado`, `PedidoFalhou`)
- Payment events (`PagamentoAprovado`, `PagamentoRecusado`)
- Inventory events (`EstoqueReservado`, `EstoqueLiberado`)
- Event replay and state reconstruction
- Analytics and audit trails

**Why Kafka:**
- Events are stored → can replay from any point in time
- Multiple consumers can read same topic independently
- High throughput for analytics
- Event sourcing: rebuild a service from scratch by replaying events

### 2. RabbitMQ — Task Queues (Background Jobs)

**Pattern:** Pub/Sub with direct queues

**Exchanges & Queues:**
| Exchange | Queue | Routing Key | Purpose |
|----------|-------|-------------|---------|
| `tasks` | `email.queue` | `send.email` | Email notifications |
| `tasks` | `invoice.queue` | `send.invoice` | Invoice generation |
| `tasks` | `sms.queue` | `send.sms` | SMS notifications |
| `tasks.dlx` | `notifications.dlq` | - | Failed messages |

**Used for:**
- Sending emails (via `WorkerEmail`)
- Generating invoices
- Sending SMS notifications
- Any "do this task once" operation

**Why RabbitMQ:**
- Built-in Dead Letter Queue (DLQ) for failed tasks
- Message acknowledgment and redelivery
- Retry logic with exponential backoff
- Simpler for task queue patterns

### 3. Synchronous Communication

**REST APIs** — Used for:
- Frontend → Service communication
- Direct service queries (when needed)
- Health checks and monitoring

**gRPC** — Used for:
- Low-latency service-to-service calls
- Internal service communication

## Event Flow

### Order Creation Flow (Kafka)

```
1. Client → POST /api/orders → PedidoService
2. PedidoService → saves to PostgreSQL
3. PedidoService → publishes "PedidoCriado" to Kafka topic "orders"
4. EstoqueService reads from "orders" topic:
   ├─ Reserves stock
   └─ publishes "EstoqueReservado" to "inventory" topic
5. PagamentoService reads from "inventory" topic:
   ├─ Processes payment (mock)
   └─ publishes "PagamentoAprovado" or "PagamentoRecusado" to "payments" topic
6. PedidoService reads from "payments" topic:
   ├─ If approved: confirms order → publishes "PedidoConfirmado" to "orders"
   └─ If refused: cancels order → publishes "PedidoFalhou" to "orders"
7. NotificacaoService reads ALL Kafka topics:
   └─ Determines notification type → dispatches to RabbitMQ
```

### Notification Flow (RabbitMQ)

```
1. NotificacaoService reads "PedidoConfirmado" from Kafka
2. Determines user wants email notification
3. Publishes to RabbitMQ:
   └─ exchange "tasks" → routing key "send.email" → queue "email.queue"
4. WorkerEmail consumes from "email.queue":
   ├─ Sends email
   ├─ On failure: retries 3 times with exponential backoff
   └─ Still fails? → moves to DLQ for manual inspection
```

### Saga Pattern (Distributed Transaction)

This project implements the **Saga Pattern** using **event-driven choreography** via Kafka:

```
Normal Flow:
  PedidoCriado → EstoqueReservado → PagamentoAprovado → PedidoConfirmado

Compensation Flow (Payment Failed):
  PagamentoRecusado → EstoqueLiberado → PedidoFalhou
```

**Key Principle:** Each service publishes events to Kafka. Other services consume from Kafka topics. There's no central coordinator.

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

### 2. Retry with Exponential Backoff (RabbitMQ)

Handles transient failures in task queues:

```
Attempt 1 → wait 1s
Attempt 2 → wait 2s
Attempt 3 → wait 4s
Give up → send to DLQ
```

### 3. Dead Letter Queue (DLQ)

Messages that fail repeatedly are moved to a DLQ for manual inspection:

```
email.queue → (3 retries failed) → tasks.dlx → notifications.dlq
```

### 4. Idempotency

Prevents duplicate processing of the same Kafka event:

```json
{
  "eventId": "unique-guid",
  "eventType": "PedidoCriado",
  "timestamp": "2026-04-06T21:00:00Z",
  "data": { ... }
}
```

Services track processed event IDs to avoid duplicates.

### 5. Kafka Consumer Offset Management

Each service manages its own offset:
- Track position in topic
- Can rewind to replay events
- Independent of other consumers

## Message Broker Comparison

### When to Use Kafka

- Event sourcing and event log
- Replay events from a specific time
- Multiple independent consumers
- Analytics and audit trails
- High throughput requirements

### When to Use RabbitMQ

- Task queues ("do this once")
- Need acknowledgment and retry
- Dead Letter Queue for failures
- Complex routing rules
- Request/response patterns

## Security Considerations

- All sensitive data in environment variables (not hardcoded)
- Database credentials managed via Docker secrets (production)
- HTTPS enabled for all REST/gRPC endpoints
- API rate limiting (future enhancement)

## Monitoring & Observability

- **Health Checks:** `/health` endpoint on each service
- **Structured Logging:** Serilog with console and file sinks
- **Kafka UI:** http://localhost:8090 (view topics, consumers, offsets)
- **RabbitMQ UI:** http://localhost:15672 (view queues, messages, DLQ)
- **Metrics:** Prometheus format (future enhancement)
- **Tracing:** Distributed tracing (future enhancement)

## Future Enhancements

- [ ] API Gateway (YARP or Ocelot)
- [ ] Distributed tracing (OpenTelemetry)
- [ ] Kubernetes deployment manifests
- [ ] CI/CD pipelines (GitHub Actions)
- [ ] Load testing and performance optimization
- [ ] Real email notifications (SendGrid/AWS SES)
- [ ] SMS worker service
