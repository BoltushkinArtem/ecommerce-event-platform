namespace Messaging.Abstractions.Messages;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class EventKeyAttribute : Attribute
{
    public string Value { get; }

    public EventKeyAttribute(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Event key cannot be empty", nameof(value));

        Value = value;
    }
}