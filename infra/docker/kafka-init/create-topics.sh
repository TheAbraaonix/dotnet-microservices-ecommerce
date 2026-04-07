#!/bin/bash
# Create Kafka topics for the e-commerce event backbone
# This script runs after Kafka starts

echo "Waiting for Kafka to be ready..."
sleep 10

echo "Creating topics..."

# Main event streaming topics
kafka-topics --create --bootstrap-server localhost:9092 \
  --topic orders --partitions 3 --replication-factor 1 \
  --config retention.ms=604800000  # 7 days retention

kafka-topics --create --bootstrap-server localhost:9092 \
  --topic payments --partitions 3 --replication-factor 1 \
  --config retention.ms=604800000

kafka-topics --create --bootstrap-server localhost:9092 \
  --topic inventory --partitions 2 --replication-factor 1 \
  --config retention.ms=604800000

kafka-topics --create --bootstrap-server localhost:9092 \
  --topic notifications --partitions 2 --replication-factor 1 \
  --config retention.ms=259200000  # 3 days retention

echo "Topics created successfully!"
echo "Listing topics:"
kafka-topics --list --bootstrap-server localhost:9092
