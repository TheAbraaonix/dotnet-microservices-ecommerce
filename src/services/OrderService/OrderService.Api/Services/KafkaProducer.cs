using System.Text.Json;
using Confluent.Kafka;

namespace PedidoService.Services;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private bool _disposed;

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        var bootstrapServers = configuration["Kafka:BootstrapServers"]
            ?? throw new InvalidOperationException("Kafka:BootstrapServers not configured");

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            MessageTimeoutMs = 5000,
            RequestTimeoutMs = 5000,
            EnableIdempotence = true,
            CompressionType = CompressionType.Lz4
        };

        _producer = new ProducerBuilder<string, string>(config).Build();

        _logger.LogInformation("Kafka producer connected to {Servers}", bootstrapServers);
    }

    public async Task PublishAsync<T>(string topic, string key, T message, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(KafkaProducer));

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            WriteIndented = false
        });

        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = json
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, kafkaMessage, ct);
            _logger.LogInformation(
                "Published event to Kafka: Topic={Topic}, Partition={Partition}, Offset={Offset}, Key={Key}",
                result.Topic, result.Partition.Value, result.Offset.Value, key);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex,
                "Failed to publish event to Kafka: Topic={Topic}, Key={Key}, Error={Error}",
                topic, key, ex.Error.Reason);
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
            _disposed = true;
        }
    }
}
