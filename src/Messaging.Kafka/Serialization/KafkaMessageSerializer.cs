using System.Text.Json;
using Messaging.Abstractions;

namespace Messaging.Kafka.Serialization;

public sealed class KafkaMessageSerializer : IKafkaMessageSerializer
{
    private readonly JsonSerializerOptions _options;

    public KafkaMessageSerializer(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public string Serialize<T>(T message)
        => JsonSerializer.Serialize(message, _options);
}