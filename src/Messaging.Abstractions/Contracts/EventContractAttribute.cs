namespace Messaging.Abstractions.Contracts;

[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class EventContractAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}