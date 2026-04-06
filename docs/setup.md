# Setup Guide

This guide will help you set up and run the e-commerce microservices project on your local machine.

## Prerequisites

Before you begin, ensure you have the following installed:

| Software | Version | Download Link |
|----------|---------|---------------|
| .NET SDK | 9.0+ | [Download](https://dotnet.microsoft.com/download) |
| Docker Desktop | Latest | [Download](https://www.docker.com/products/docker-desktop/) |
| Node.js | 20+ | [Download](https://nodejs.org/) |
| Git | Latest | [Download](https://git-scm.com/) |

## Quick Start

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd microservices
```

### 2. Start Infrastructure Services

This command starts all databases, message brokers, and monitoring tools:

```bash
docker-compose -f infra/docker/docker-compose.yml up -d
```

**What this starts:**

| Service | Port | Purpose |
|---------|------|---------|
| PostgreSQL | 5432 | Primary relational database |
| MongoDB | 27017 | Document database (order history) |
| Redis | 6379 | Caching layer |
| RabbitMQ | 5672, 15672 | Message broker (AMQP) |
| Kafka | 9092, 29092 | Event streaming |
| Kafka UI | 8090 | Kafka web interface |
| Zookeeper | 2181 | Kafka dependency |

**Access Management UIs:**

- **RabbitMQ:** http://localhost:15672 (guest/guest)
- **Kafka UI:** http://localhost:8090

### 3. Verify Infrastructure

Check that all services are healthy:

```bash
docker-compose -f infra/docker/docker-compose.yml ps
```

### 4. Run a Microservice Locally

Navigate to a service directory and run:

```bash
cd src/services/PedidoService/src
dotnet run
```

The service will start on:
- **HTTP:** http://localhost:5001
- **Swagger UI:** http://localhost:5001/swagger
- **Health Check:** http://localhost:5001/health

### 5. Run Tests

```bash
cd src/services/PedidoService
dotnet test
```

## Database Setup

PostgreSQL is automatically initialized with the following databases:

| Database | Purpose |
|----------|---------|
| `pedidos` | Order management |
| `estoque` | Inventory tracking |
| `pagamentos` | Payment processing |
| `notificacoes` | Notification audit trail |
| `ecommerce` | Main database with schemas |

### Connect to PostgreSQL

```bash
# Using psql
psql -h localhost -U admin -d pedidos

# Using Docker
docker exec -it ecommerce-postgres psql -U admin -d pedidos
```

### Connect to MongoDB

```bash
# Using mongosh
mongosh "mongodb://admin:admin123@localhost:27017"

# Using Docker
docker exec -it ecommerce-mongodb mongosh -u admin -p admin123
```

## Configuration

### Environment Variables

Each service reads configuration from `appsettings.json` and `appsettings.Development.json`:

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__Default` | Database connection string | Local PostgreSQL |
| `RabbitMQ__Host` | RabbitMQ server | localhost |
| `RabbitMQ__Port` | RabbitMQ port | 5672 |
| `RabbitMQ__Username` | RabbitMQ user | guest |
| `RabbitMQ__Password` | RabbitMQ password | guest |

### Overriding Configuration

Use environment variables to override settings:

```bash
# Windows (PowerShell)
$env:ConnectionStrings__Default = "Host=db-server;Database=pedidos;..."
dotnet run

# Linux/macOS
export ConnectionStrings__Default="Host=db-server;Database=pedidos;..."
dotnet run
```

## Running with Docker (Services)

Once services are built, you can run everything with Docker Compose:

```bash
# Uncomment service definitions in docker-compose.yml
docker-compose -f infra/docker/docker-compose.yml up -d
```

## Troubleshooting

### Port Already in Use

If a port is already in use, find and kill the process:

```bash
# Windows
netstat -ano | findstr :5432
taskkill /PID <PID> /F

# Linux/macOS
lsof -i :5432
kill -9 <PID>
```

### Docker Issues

```bash
# Restart Docker Desktop
# Or clean up unused containers/images
docker system prune -a
```

### Service Won't Start

1. Check logs: `docker logs <container-name>`
2. Verify infrastructure is healthy: `docker-compose ps`
3. Check connection strings in appsettings.json

## Next Steps

- Read the [Architecture Documentation](docs/architecture.md)
- Explore [Architecture Decision Records](docs/adr/)
- Check individual service README files
