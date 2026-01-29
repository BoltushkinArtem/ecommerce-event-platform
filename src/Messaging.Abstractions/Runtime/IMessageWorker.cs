namespace Messaging.Abstractions.Runtime;

public interface IMessageWorker
{
    Task RunAsync(CancellationToken cancellationToken);
}