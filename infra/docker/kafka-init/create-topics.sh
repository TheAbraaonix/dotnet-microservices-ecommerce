#!/bin/bash
# Create Kafka topics for the e-commerce event backbone
# Run this AFTER Kafka is healthy (healthcheck passed)
# Usage: docker exec ecommerce-kafka /kafka-init/create-topics.sh

echo "=== Kafka Topic Initialization ==="
echo "Waiting for Kafka to be ready..."
sleep 10

BOOTSTRAP="localhost:9092"

# Check if topics already exist
EXISTING=$(kafka-topics --list --bootstrap-server $BOOTSTRAP 2>/dev/null)

create_topic_if_not_exists() {
    local topic=$1
    local partitions=$2
    local retention_ms=$3
    
    if echo "$EXISTING" | grep -q "^${topic}$"; then
        echo "✅ Topic '${topic}' already exists, skipping"
    else
        echo "📝 Creating topic '${topic}' (${partition} partitions, retention: $((retention_ms/86400000)) days)"
        kafka-topics --create \
            --bootstrap-server $BOOTSTRAP \
            --topic "$topic" \
            --partitions "$partitions" \
            --replication-factor 1 \
            --config retention.ms="$retention_ms" \
            --config cleanup.policy=delete
        echo "✅ Topic '${topic}' created"
    fi
}

echo ""
echo "Checking/creating topics..."
echo ""

# Main event streaming topics
create_topic_if_not_exists "orders"        3  604800000   # 7 days retention
create_topic_if_not_exists "payments"      3  604800000   # 7 days retention
create_topic_if_not_exists "inventory"     2  604800000   # 7 days retention
create_topic_if_not_exists "notifications" 2  259200000   # 3 days retention

echo ""
echo "=== Topics Summary ==="
kafka-topics --describe --bootstrap-server $BOOTSTRAP
echo ""
echo "=== Initialization Complete ==="
