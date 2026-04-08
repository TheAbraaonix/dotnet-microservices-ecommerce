# QWEN.md - Microservices & Event-Driven Architecture Learning Project

## Project Overview

This is a **portfolio-ready e-commerce microservices project** designed for learning distributed systems and event-driven architecture. The project demonstrates the transition from junior to mid-level developer skills through hands-on implementation of real-world microservices patterns.

### Purpose
Build a complete e-commerce platform with order management, inventory tracking, payment processing, and notification systems using modern cloud-native technologies.

### Architecture
Event-driven microservices with both synchronous (REST/gRPC) and asynchronous (RabbitMQ/Kafka) communication patterns.

## Technology Stack

| Layer | Technologies |
|-------|-------------|
| **Backend** | .NET 8 LTS (ASP.NET Core) |
| **Frontend** | Angular 20+ (TypeScript) |
| **Message Brokers** | Kafka (event backbone), RabbitMQ (task queues) |
| **Databases** | PostgreSQL (relational), MongoDB (document store), Redis (caching) |
| **Containerization** | Docker, Docker Compose, Kubernetes |
| **Monitoring** | Serilog (logging), Prometheus (metrics) |
| **Resilience** | Polly (circuit breaker, retry patterns) |

## Microservices Structure

### Core Services

1. **PedidoService** (Order Service)
   - REST + gRPC APIs
   - PostgreSQL (orders) + MongoDB (history/analytics)
   - Events: `PedidoCriado`, `PedidoConfirmado`, `PedidoFalhou`
   - Subscribes: `PagamentoAprovado`, `EstoqueReservado`

2. **EstoqueService** (Inventory Service)
   - REST + gRPC APIs
   - PostgreSQL
   - Events: `EstoqueReservado`, `EstoqueInsuficiente`
   - Implements: Saga pattern with compensation logic

3. **PagamentoService** (Payment Service)
   - Mock payment processing
   - REST + gRPC APIs
   - PostgreSQL
   - Events: `PagamentoAprovado`, `PagamentoRecusado`
   - Implements: Retry logic, idempotency keys

4. **NotificacaoService** (Notification Service)
   - Subscribes to all events
   - PostgreSQL (audit trail) + Redis (caching)
   - Simulated notifications → real emails

5. **WorkerEmail** (Background Worker)
   - RabbitMQ job consumer
   - Handles: `SendEmail`, `SendInvoice`
   - Implements: Dead Letter Queue (DLQ) pattern

### Key Patterns
- **Saga Pattern**: Distributed transaction coordination (choreography via Kafka events)
- **Retry/DLQ**: Graceful message failure handling (RabbitMQ task queues)
- **Idempotency**: Prevent duplicate processing
- **Circuit Breaker**: Resilience with Polly
- **Timeout Handling**: Service call timeouts
- **Event Replay**: Kafka offset management for state reconstruction

## Project Phases

1. **Phase 1**: Foundation & Docker setup
2. **Phase 2**: Core .NET microservices implementation
3. **Phase 3**: Angular frontend (Order/Inventory dashboards, Admin panel)
4. **Phase 4**: Message broker mastery (RabbitMQ + Kafka)
5. **Phase 5**: Containerization & Kubernetes orchestration
6. **Phase 6**: Production-ready features (monitoring, resilience, documentation)
7. **Phase 7**: Portfolio polish & GitHub setup

## Building and Running

*Note: This project is in the planning stage. Implementation commands will be added as development progresses.*

### Expected Commands (once implemented)

```bash
# Run all services locally
docker-compose up -d

# Build individual service
dotnet build src/PedidoService

# Run individual service
dotnet run --project src/PedidoService

# Run Angular frontend
cd src/Frontend && ng serve

# Run tests
dotnet test

# Kubernetes deployment (future)
kubectl apply -f k8s/
```

## Key Files

| File | Description |
|------|-------------|
| `plan.md` | Comprehensive learning plan with 7 phases, timeline, and success criteria |
| `microservice_project_idea_scope.txt` | Initial project scope in Portuguese, outlining core services and learning goals |

## Development Guidelines

### Backend (.NET)
- Use ASP.NET Core for REST APIs
- Implement gRPC for service-to-service communication
- Apply MassTransit or EasyNetQ for messaging
- Use Serilog for structured logging
- Implement Polly for resilience patterns

### Frontend (Angular)
- Use HttpClientModule for API communication
- Implement RxJS for async operations
- Add real-time updates (SignalR optional)
- Proper error handling and loading states

### Testing
- Unit tests for business logic
- Integration tests for API endpoints
- Message broker integration tests
- Docker Compose for end-to-end testing

### Docker
- Multi-stage builds for optimized images
- Proper `.dockerignore` files
- Network configuration for service communication
- Volume management for databases

## Success Criteria

By project completion, you should be able to:
- Explain microservices vs monolithic architecture
- Design distributed systems with event-driven patterns
- Troubleshoot message broker issues
- Deploy and manage containerized services
- Understand Kubernetes concepts and manifests
- Implement resilience patterns in production
- Build scalable REST and gRPC APIs
- Work with multiple databases appropriately

## Current Status

**Phase**: Planning (Phase 0)

The project is currently in the planning stage with detailed documentation. No code has been implemented yet. The next step is to begin Phase 1: Foundation & Environment Setup (Docker configuration, message broker setup, database decisions).

## Notes

- Timeline is flexible: ~16 weeks suggested, focus on understanding over speed
- Designed as a portfolio piece showcasing growth from junior → mid-level engineer
- Both RabbitMQ and Kafka included to learn when to use each message broker
- **Kafka**: Event backbone for business events (order flow, payments, inventory)
- **RabbitMQ**: Task queues for background jobs (email, SMS, invoices)
- E-commerce scope chosen for being relatable and demonstrating real-world patterns
