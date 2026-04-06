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
           │         │  RabbitMQ /     │         │
           └────────►│     Kafka       │◄────────┘
                     │  (Event Bus)    │
           ┌────────►│                 │◄────────┐
           │         └────────┬────────┘         │
           │                  │                  │
┌──────────▼───────────────┐  │  ┌───────────────▼───────────────┐
│  PagamentoService        │  │  │  NotificacaoService           │
│  (Payment - Mock)        │  │  │  (Notifications)              │
│  PostgreSQL              │  │  │  PostgreSQL + Redis           │
└──────────────────────────┘  │  └───────────────────────────────┘
                              │
                     ┌────────▼────────┐
                     │  WorkerEmail    │
                     │  (Background)   │
                     └─────────────────┘
```

### Services

| Service | Description | Database | APIs |
|---------|-------------|----------|------|
| **PedidoService** | Order creation and management | PostgreSQL + MongoDB | REST + gRPC |
| **EstoqueService** | Inventory tracking and reservation | PostgreSQL | REST + gRPC |
| **PagamentoService** | Mock payment processing | PostgreSQL | REST + gRPC |
| **NotificacaoService** | Event notifications and audit | PostgreSQL + Redis | REST |
| **WorkerEmail** | Background email processing | - | RabbitMQ Consumer |

### Technology Stack

- **Backend:** .NET 8+ (ASP.NET Core)
- **Frontend:** Angular 20+
- **Message Brokers:** RabbitMQ (pub/sub), Kafka (event streaming)
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

### Order Creation Flow

1. **Client** creates order via Angular → `PedidoService`
2. **PedidoService** publishes `PedidoCriado` event
3. **EstoqueService** consumes event → reserves stock → publishes `EstoqueReservado`
4. **PagamentoService** consumes `EstoqueReservado` → processes payment → publishes `PagamentoAprovado`
5. **PedidoService** consumes `PagamentoAprovado` → confirms order → publishes `PedidoConfirmado`
6. **NotificacaoService** consumes all events → sends notifications

### Saga Pattern (Compensation)

If payment fails:
1. **PagamentoService** publishes `PagamentoRecusado`
2. **EstoqueService** consumes event → releases reserved stock
3. **PedidoService** marks order as failed → publishes `PedidoFalhou`

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
- RabbitMQ Management: `http://localhost:15672` (guest/guest)

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
