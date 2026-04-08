# E-Commerce Microservices

A portfolio-ready e-commerce platform built with **event-driven microservices architecture** using .NET 8+, Angular, Docker, and message brokers (RabbitMQ + Kafka).

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Angular Frontend                         │
└──────────────┬──────────────────────────────────────┬───────────┘
               │ REST API                             │ REST API
               ▼                                      ▼
┌──────────────────────────┐          ┌───────────────────────────────┐
│   PedidoService          │          │   EstoqueService              │
│   (Order Management)     │          │   (Inventory)                 │
│   PostgreSQL + MongoDB   │          │   PostgreSQL                  │
└──────────┬───────────────┘          └──────────┬────────────────────┘
           │                                     │
           │         ┌─────────────────┐         │
           │         │     Kafka       │         │
           └────────►│  (Event Log)    │◄────────┘
                     │                 │
           ┌────────►│                 │◄────────┐
           │         └────────┬────────┘         │
           │                  │                  │
┌──────────▼───────────────┐  │  ┌───────────────▼───────────────┐
│  PagamentoService        │  │  │  NotificacaoService           │
│  (Payment - Mock)        │  │  │  (Reads Kafka → sends tasks)  │
│  PostgreSQL              │  │  │  PostgreSQL + Redis           │
└──────────────────────────┘  │  └──────────┬────────────────────┘
                              │             │
                              │             │ RabbitMQ tasks
                     ┌────────▼────────┐    │
                     │  WorkerEmail    │◄───┘
                     │  (Background)   │
                     └─────────────────┘
```

### Services

| Service | Description | Database | APIs |
|---------|-------------|----------|------|
| **PedidoService** | Order creation and management | PostgreSQL + MongoDB | REST + gRPC |
| **EstoqueService** | Inventory tracking and reservation | PostgreSQL | REST + gRPC |
| **PagamentoService** | Mock payment processing | PostgreSQL | REST + gRPC |
| **NotificacaoService** | Reads Kafka events, dispatches tasks to RabbitMQ | PostgreSQL + Redis | REST |
| **WorkerEmail** | Background email/SMS processing (consumes RabbitMQ) | - | RabbitMQ Consumer |

### Technology Stack

- **Backend:** .NET 8 LTS (ASP.NET Core)
- **Frontend:** Angular 20+
- **Event Backbone:** Kafka (order flow, payments, inventory events)
- **Task Queues:** RabbitMQ (email, SMS, invoices, notifications)
- **Databases:** PostgreSQL, MongoDB, Redis
- **Containerization:** Docker, Docker Compose, Kubernetes
- **Resilience:** Polly (circuit breaker, retry)
- **Logging:** Serilog (structured logging)

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Node.js 20+](https://nodejs.org/) (for Angular frontend)
- [Angular CLI](https://angular.io/cli): `npm install -g @angular/cli`

### Running with Docker Compose

Start all infrastructure and services:

```bash
docker-compose -f infra/docker/docker-compose.yml up -d
```

Stop all services:

```bash
docker-compose -f infra/docker/docker-compose.yml down
```

### Running Services Locally

```bash
# Run PedidoService
dotnet run --project src/services/PedidoService/src

# Run EstoqueService
dotnet run --project src/services/EstoqueService/src

# Run Angular Frontend
cd src/frontend
ng serve
```

## 📁 Project Structure

```
microservices/
├── src/
│   ├── services/
│   │   ├── PedidoService/        # Order management
│   │   ├── EstoqueService/       # Inventory control
│   │   ├── PagamentoService/     # Payment processing
│   │   ├── NotificacaoService/   # Notifications
│   │   └── WorkerEmail/          # Email background worker
│   └── frontend/                 # Angular application
├── infra/
│   └── docker/                   # Docker Compose & config
├── docs/
│   └── adr/                      # Architecture Decision Records
├── scripts/                      # Helper scripts
├── .github/workflows/            # CI/CD pipelines
└── README.md
```

## 📖 Documentation

- [Setup Guide](docs/setup.md) - Detailed setup instructions
- [Architecture](docs/architecture.md) - Service communication patterns
- [Architecture Decision Records](docs/adr/) - Why we chose each technology

## 🔄 Event Flow

### Order Creation Flow (via Kafka)

1. **Client** creates order via Angular → `PedidoService`
2. **PedidoService** saves order → publishes `PedidoCriado` to Kafka topic `orders`
3. **EstoqueService** reads from `orders` topic → reserves stock → publishes `EstoqueReservado` to `inventory` topic
4. **PagamentoService** reads from `inventory` topic → processes payment → publishes `PagamentoAprovado` to `payments` topic
5. **PedidoService** reads from `payments` topic → confirms order → publishes `PedidoConfirmado` to `orders` topic
6. **NotificacaoService** reads ALL Kafka topics → dispatches tasks to RabbitMQ queues

### Saga Pattern (Compensation)

If payment fails:
1. **PagamentoService** publishes `PagamentoRecusado` to `payments` topic
2. **EstoqueService** reads event → releases reserved stock → publishes `EstoqueLiberado`
3. **PedidoService** reads event → marks order as failed → publishes `PedidoFalhou`

### Notification Flow (via RabbitMQ)

1. **NotificacaoService** reads Kafka events
2. Determines user notification needed (email, SMS)
3. Publishes task to RabbitMQ:
   - `send.email` → `email.queue` → `WorkerEmail` consumes
   - `send.sms` → `sms.queue` → `WorkerSMS` consumes (future)
4. Workers process tasks with retry/DLQ handling

## 🛠️ Development

### Key Patterns Implemented

- **Saga Pattern** - Distributed transaction coordination (choreography)
- **Retry / DLQ** - Graceful message failure handling
- **Idempotency** - Prevent duplicate processing
- **Circuit Breaker** - Resilience with Polly
- **Dead Letter Queue** - Failed message handling

### Running Tests

```bash
dotnet test
```

### Building Docker Images

```bash
docker build -t pedidoservice src/services/PedidoService
```

## 📊 Monitoring

- Health checks: `http://localhost:{port}/health`
- Swagger UI: `http://localhost:{port}/swagger`
- RabbitMQ Management: `http://localhost:15672` (guest/guest) — task queues, DLQ
- Kafka UI: `http://localhost:8090` — event topics, consumer offsets

## 🎓 Learning Goals

This project demonstrates:
- ✅ Microservices architecture patterns
- ✅ Event-driven design
- ✅ Synchronous (REST/gRPC) + Asynchronous (messaging) communication
- ✅ Database diversity (SQL + NoSQL)
- ✅ Docker containerization
- ✅ Kubernetes basics
- ✅ Full-stack development
- ✅ Production-ready practices

## 📝 Status

**Current Phase:** Phase 1 - Foundation & Infrastructure

See [plan.md](plan.md) for the complete learning roadmap.

## 📄 License

MIT
