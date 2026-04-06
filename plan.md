# Microservices & Event-Driven Architecture Learning Plan

## Overview
Build a portfolio-ready e-commerce microservices project using .NET (backend), Angular (frontend), Docker, and event-driven messaging. This plan balances practical learning with portfolio impact while maintaining manageable complexity for steady-paced learning.

---

## Project Validation

✅ **YES, this is an excellent learning project for you because:**
- E-commerce scope is relatable and demonstrates real-world patterns
- Forces you to master event-driven architecture (core microservices skill)
- Teaches async messaging, retry logic, and error handling
- Low enough complexity to complete, high enough to showcase advanced knowledge
- Perfect portfolio piece for mid-level developer growth

---

## Phase 1: Foundation & Environment Setup

### Learning Goals
- Understand microservices principles
- Set up Docker and containerization
- Configure local development environment
- Plan service architecture

### Key Deliverables
1. Docker & Docker Compose setup
   - Containerize each service
   - Network configuration for service communication
   - Volume management for databases

2. Database decisions
   - **PedidoService**: SQL (PostgreSQL) for orders + NoSQL (MongoDB) for order history/analytics
   - **EstoqueService**: SQL (PostgreSQL) for inventory
   - **PagamentoService**: SQL (PostgreSQL) for transactions
   - **NotificacaoService**: Redis for caching, PostgreSQL for audit trail

3. Message broker setup (local)
   - RabbitMQ (for simple pub/sub messaging)
   - Kafka (for event streaming and event log)
   - Both running in Docker containers
   - Understanding when to use each

4. Documentation
   - Architecture diagram (services, dependencies, message flows)
   - Setup guide (how to run everything locally)

---

## Phase 2: Core Microservices Implementation (.NET)

### Services to Build

#### 1. **PedidoService** (Order Service)
- REST API (Create, Get, List orders)
- gRPC API (fast service-to-service calls)
- Events published: `PedidoCriado`, `PedidoConfirmado`, `PedidoFalhou`
- Subscribes to: `PagamentoAprovado`, `EstoqueReservado`
- Database: PostgreSQL + MongoDB

#### 2. **EstoqueService** (Inventory Service)
- REST & gRPC APIs
- Events published: `EstoqueReservado`, `EstoqueInsuficiente`
- Subscribes to: `PedidoCriado`, `PagamentoAprovado`
- Database: PostgreSQL
- Saga pattern: Implement compensation logic (if payment fails, release reserved stock)

#### 3. **PagamentoService** (Payment Service)
- Mock payment processing (no real payments)
- REST & gRPC APIs
- Events published: `PagamentoAprovado`, `PagamentoRecusado`
- Subscribes to: `PedidoCriado`, `EstoqueReservado`
- Database: PostgreSQL
- Implement retry logic and idempotency keys

#### 4. **NotificacaoService** (Notification Service)
- Subscribes to: All order/payment/stock events
- Sends notifications (simulated console output first, then real emails)
- Database: PostgreSQL for audit trail
- Redis for caching recent notifications

#### 5. **WorkerEmail** (Background Worker)
- Consumes from RabbitMQ jobs queue
- Handles: `SendEmail`, `SendInvoice`
- Demonstrates long-running background tasks
- Implements retry with Dead Letter Queue (DLQ)

### Key Patterns to Implement
- **Saga Pattern**: Distributed transaction coordination (Choreography style via events)
- **Retry/DLQ**: Handle message failures gracefully
- **Idempotency**: Prevent duplicate processing
- **Async communication**: Event publishing and consumption
- **Timeout handling**: Service call timeouts and circuit breakers

---

## Phase 3: Frontend Implementation (Angular)

### Features
1. **Order Management Dashboard**
   - Create new orders
   - View order history and status
   - Real-time status updates (SignalR or polling)
   - Order details page

2. **Inventory Dashboard**
   - View current stock
   - Basic inventory adjustments (admin only)

3. **Admin Panel**
   - Monitor all services
   - View message broker metrics
   - Retry failed messages manually
   - Service health dashboard

### Integration Points
- REST APIs for typical CRUD operations
- Real-time updates via SignalR (optional enhancement)
- Error handling and user feedback
- Loading states for async operations

---

## Phase 4: Message Broker Mastery

### RabbitMQ Implementation
1. Topic-based exchanges for events
2. Multiple queues for different services
3. Dead Letter Exchange (DLQ) for failed messages
4. Message acknowledgment and redelivery

### Kafka Implementation
1. Topics: Orders, Payments, Inventory, Notifications
2. Consumer groups for different services
3. Partition strategy
4. Offset management
5. Event log replay capability

### Comparison & Documentation
- When to use RabbitMQ (simple, task queues)
- When to use Kafka (event streaming, high throughput)
- Performance testing between both
- Document trade-offs in your portfolio

---

## Phase 5: Containerization & Orchestration

### Docker
1. Create Dockerfiles for each service
2. Optimize images (multi-stage builds)
3. Docker Compose configuration
   - All services, databases, and brokers
   - Network configuration
   - Volume management
   - Environment variables
4. `.dockerignore` files

### Kubernetes Basics
1. Minikube or kind setup (local Kubernetes)
2. Convert Docker Compose to Kubernetes manifests (YAML)
   - Deployments for each service
   - Services for internal communication
   - ConfigMaps for configuration
   - Secrets for sensitive data
3. Helm charts (optional) for templating
4. Local testing in Kubernetes

---

## Phase 6: Production-Ready Features

### Monitoring & Logging
- Structured logging (Serilog in .NET)
- Centralized log aggregation (ELK Stack optional, or simple file logging)
- Service health checks
- Metrics exposure (Prometheus format)

### Resilience Patterns
- Circuit breaker (Polly library in .NET)
- Timeout handling
- Bulkhead isolation
- Graceful degradation

### API Documentation
- Swagger/OpenAPI for REST APIs
- gRPC documentation
- Architecture decision records (ADR)

---

## Phase 7: Portfolio Polish & GitHub

### Documentation
1. **README.md**
   - Project overview
   - How to run (Docker Compose + Kubernetes)
   - Architecture diagram
   - Service endpoints

2. **Architecture Documentation**
   - Service-to-service communication patterns
   - Event flow diagrams
   - Database schemas
   - Decision log (why RabbitMQ + Kafka, why gRPC, etc.)

3. **Setup Guide**
   - Prerequisites
   - Step-by-step setup instructions
   - Troubleshooting section

### GitHub Repository
- Clean commit history with meaningful messages
- .gitignore for .NET and Node projects
- CI/CD pipeline (GitHub Actions) - optional but impressive
  - Build on push
  - Run basic tests
  - Generate Docker images

### LinkedIn Portfolio Piece
- Screenshot of architecture diagram
- Brief description of technologies used
- Link to GitHub repo
- Highlight: "Built microservices with event-driven architecture using .NET, Angular, RabbitMQ, Kafka, and Docker/Kubernetes"

---

## Learning Progression (By Do-First Order)

1. **Week 1-2**: Docker setup + Phase 1 foundations
2. **Week 3-4**: Build PedidoService (simplest)
3. **Week 5-6**: Build EstoqueService + event integration
4. **Week 7-8**: Build PagamentoService + saga pattern
5. **Week 9-10**: Build NotificacaoService + WorkerEmail
6. **Week 11-12**: Angular frontend
7. **Week 13-14**: Kafka + advanced messaging patterns
8. **Week 15-16**: Kubernetes + Polish

*Timeline is flexible per your pace. Focus on understanding, not speed.*

---

## Technology Stack Summary

**Backend**: .NET 8+ (ASP.NET Core)
- Libraries: MassTransit or EasyNetQ (messaging), Polly (resilience), Serilog (logging)

**Frontend**: Angular 20+ (TypeScript)
- Libraries: HttpClientModule, RxJS for async

**Messaging**: RabbitMQ (primary) + Kafka (secondary for learning)

**Databases**: PostgreSQL (relational), MongoDB (document store), Redis (caching)

**Containerization**: Docker, Docker Compose, Kubernetes (minikube/kind)

**Infrastructure**: GitHub (version control), Docker Hub (optional image registry)

---

## Portfolio Impact

This project demonstrates:
✅ Microservices architecture understanding
✅ Event-driven design patterns
✅ Both synchronous (REST/gRPC) and asynchronous (messaging) communication
✅ Database diversity (SQL + NoSQL)
✅ Docker containerization expertise
✅ Kubernetes basics
✅ Full-stack development (backend + frontend)
✅ Professional coding practices (logging, error handling, documentation)
✅ Production-ready thinking (resilience, monitoring, deployment)

Perfect for showcasing growth from junior → mid-level engineer.

---

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
