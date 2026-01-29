using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Runtime.Hosting.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageWorkerHostedRuntime(
        this IServiceCollection services)
    {
        services.AddHostedService<HostedMessageWorker>();
        return services;
    }
}