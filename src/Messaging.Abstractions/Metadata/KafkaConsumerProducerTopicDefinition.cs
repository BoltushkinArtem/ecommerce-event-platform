namespace Messaging.Abstractions.Metadata;

public sealed record KafkaConsumerTopicDefinition(Type Event, string Name);