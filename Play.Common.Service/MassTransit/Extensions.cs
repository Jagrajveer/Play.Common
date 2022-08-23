using System.Diagnostics;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Service.Settings;

namespace Play.Common.Service.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddMassTransit(configurator =>
        {
            configurator.AddConsumers(Assembly.GetEntryAssembly());
            
            configurator.UsingRabbitMq(((context, factoryConfigurator) =>
            {
                var configuration = context.GetService<IConfiguration>();
                Debug.Assert(configuration != null, nameof(configuration) + " != null");
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var rabbitMqSettings = configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
                factoryConfigurator.Host(rabbitMqSettings.Host);
                factoryConfigurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                factoryConfigurator.UseMessageRetry(retryConfigurator =>
                {
                    retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                });
            }));
        });

        return serviceCollection;
    }
}
