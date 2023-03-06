using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Common.Toolkits.Workers
{
    public static class HostWorkerCollectionExtension
    {
        public static IServiceCollection AddHostedWorker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THostedService>(this IServiceCollection services) where THostedService : class, IHostedWorker
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedWorker, THostedService>());
            return services;
        }

        public static IServiceCollection AddHostedWorker<THostedService>(this IServiceCollection services, Func<IServiceProvider, THostedService> implementationFactory) where THostedService : class, IHostedWorker
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton((Func<IServiceProvider, IHostedWorker>)implementationFactory));
            return services;
        }
    }
}
