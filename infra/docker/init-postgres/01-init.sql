-- Initialize PostgreSQL databases for each microservice
-- This script runs on first container startup

-- Create databases
CREATE DATABASE pedidos;
CREATE DATABASE estoque;
CREATE DATABASE pagamentos;
CREATE DATABASE notificacoes;

-- Create schemas within ecommerce database
\c ecommerce

-- Schema for order-related tables (if needed in the main db)
CREATE SCHEMA IF NOT EXISTS orders;

-- Schema for inventory-related tables
CREATE SCHEMA IF NOT EXISTS inventory;

-- Schema for payment-related tables
CREATE SCHEMA IF NOT EXISTS payments;

-- Schema for notification-related tables
CREATE SCHEMA IF NOT EXISTS notifications;

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE pedidos TO admin;
GRANT ALL PRIVILEGES ON DATABASE estoque TO admin;
GRANT ALL PRIVILEGES ON DATABASE pagamentos TO admin;
GRANT ALL PRIVILEGES ON DATABASE notificacoes TO admin;
