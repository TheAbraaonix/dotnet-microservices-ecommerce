# Architecture Decision Records

ADRs document the why behind technical decisions in this project.

---

## ADR-001: Monorepo Structure

**Status:** Accepted  
**Date:** 2026-04-06

### Context

We needed to decide between a monorepo (single repository) or multi-repo (one per service) approach.

### Decision

Use a **monorepo** structure with all services in a single repository.

### Consequences

**Pros:**
- Easier to manage during learning phase
- Single CI/CD pipeline
- Simplified portfolio presentation
- Atomic commits across services
- Easier dependency management

**Cons:**
- Not suitable for large teams with independent deployment cycles
- Repository grows larger over time
- Less realistic for enterprise production environments

**Note:** In production environments with multiple teams, multi-repo would be preferred. This decision is optimal for learning and portfolio purposes.

---

## ADR-002: Kafka as Event Backbone, RabbitMQ for Task Queues

**Status:** Accepted  
**Date:** 2026-04-06

### Context

We needed to decide how to use both Kafka and RabbitMQ in our architecture. Initially, we considered RabbitMQ as the primary event bus for all service-to-service communication. After review, we decided to follow the industry-standard pattern used by mature companies (Uber, Netflix, LinkedIn).

### Decision

Use **Kafka as the event backbone** for all business events (order flow, payments, inventory) and **RabbitMQ for task queues** (email, SMS, invoice generation, background jobs).

### Consequences

**Kafka (Event Backbone):**
- Stores all business events with retention (7 days)
- Enables event replay — can rebuild a service from scratch
- Multiple independent consumers can read same topic
- High throughput for analytics
- Used for: `PedidoCriado`, `PagamentoAprovado`, `EstoqueReservado`, etc.

**RabbitMQ (Task Queues):**
- "Do this task once" pattern
- Built-in Dead Letter Queue for failed tasks
- Message acknowledgment and retry logic
- Used for: `SendEmail`, `SendSMS`, `GenerateInvoice`

**Communication Flow:**
1. Business events flow through Kafka (order lifecycle, payments, inventory)
2. NotificacaoService reads Kafka events → determines notification type
3. Dispatches tasks to RabbitMQ queues (email, SMS, etc.)
4. Workers (WorkerEmail, WorkerSMS) consume from RabbitMQ with retry/DLQ

**Learning Value:** Understand the strengths of each broker and when to use them. This pattern mirrors production systems at scale.

---

## ADR-003: PostgreSQL as Primary Database

**Status:** Accepted  
**Date:** 2026-04-06

### Context

We needed to choose a primary relational database for the services.

### Decision

Use **PostgreSQL** as the primary database for all services that need relational storage.

### Consequences

**Pros:**
- Excellent .NET support via Npgsql
- Strong ACID compliance
- JSON support for flexible data
- Free and open source
- Docker-friendly

**Cons:**
- Not as commonly used in .NET enterprise environments as SQL Server
- Requires separate database per service (managed via Docker Compose)

---

## ADR-004: .NET 9 for Backend

**Status:** Accepted  
**Date:** 2026-04-06

### Context

Choose the backend framework and version.

### Decision

Use **.NET 9** (latest stable) for all microservices.

### Consequences

**Pros:**
- Latest performance improvements
- Built-in OpenAPI support
- Minimal APIs option
- Strong ecosystem (MassTransit, Polly, Serilog)
- Cross-platform

**Cons:**
- May not be available on all cloud providers yet
- Some enterprise environments still on .NET 6/8

---

## ADR-005: MassTransit for Messaging

**Status:** Accepted  
**Date:** 2026-04-06

### Context

We needed a messaging abstraction layer for .NET.

### Decision

Use **MassTransit** as the messaging framework for RabbitMQ integration.

### Consequences

**Pros:**
- Clean abstractions over RabbitMQ/Kafka
- Built-in retry, DLQ, and saga support
- Easy to test
- Industry standard for .NET messaging

**Cons:**
- Learning curve for configuration
- Adds dependency to all services
