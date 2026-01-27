namespace Messaging.Abstractions;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EventContractAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}