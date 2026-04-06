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

## ADR-002: RabbitMQ + Kafka (Dual Message Brokers)

**Status:** Accepted  
**Date:** 2026-04-06

### Context

Most projects choose one message broker. We decided to include both RabbitMQ and Kafka.

### Decision

Use **RabbitMQ** for pub/sub messaging and task queues, and **Kafka** for event streaming and event log.

### Consequences

**RabbitMQ (Primary):**
- Simpler queue management
- Better for task queues (email jobs)
- Built-in Dead Letter Exchange
- Easier to learn and configure

**Kafka (Secondary):**
- Event streaming and replay capability
- Higher throughput for analytics
- Event sourcing patterns
- Industry standard for event log

**Learning Value:** Understand when to use each broker and their trade-offs.

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
