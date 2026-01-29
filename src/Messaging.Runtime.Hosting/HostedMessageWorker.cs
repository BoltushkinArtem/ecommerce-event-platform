using Messaging.Abstractions.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Runtime.Hosting;

public sealed class HostedMessageWorker(
    IMessageWorker worker,
    ILogger<HostedMessageWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Message worker started");
        await worker.RunAsync(stoppingToken);
        logger.LogInformation("Message worker stopped");
    }
}