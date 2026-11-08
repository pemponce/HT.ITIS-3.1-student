using Dotnet.Homeworks.MainProject.Configuration;
using MassTransit;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        RabbitMqConfig rabbitConfiguration)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitConfiguration.Hostname, "/", host =>
                {
                    host.Username(rabbitConfiguration.Username);
                    host.Password(rabbitConfiguration.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
