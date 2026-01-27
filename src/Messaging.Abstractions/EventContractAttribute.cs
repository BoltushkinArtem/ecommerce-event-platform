namespace Messaging.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class EventContractAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}