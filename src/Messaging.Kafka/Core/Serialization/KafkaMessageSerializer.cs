using System.Text.Json;

namespace Messaging.Kafka.Core.Serialization;

public sealed class KafkaMessageSerializer(JsonSerializerOptions? options = null) : IKafkaMessageSerializer
{
    private readonly JsonSerializerOptions _options = options ?? new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public string Serialize<T>(T message)
        => JsonSerializer.Serialize(message, _options);

    public object Deserialize(string payload, Type messageType)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Message payload is empty", nameof(payload));

        var result = JsonSerializer.Deserialize(payload, messageType, _options);

        return result
               ?? throw new InvalidOperationException(
                   $"Failed to deserialize message to type {messageType.Name}");
    }
}