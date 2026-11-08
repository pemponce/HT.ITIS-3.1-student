using Dotnet.Homeworks.Mailing.API.Configuration;
using Dotnet.Homeworks.Mailing.API.Helpers;
using MassTransit;

namespace Dotnet.Homeworks.Mailing.API.ServicesExtensions;

public static class AddMasstransitRabbitMqExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        RabbitMqConfig rabbitConfiguration)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumers(AssemblyReference.Assembly);

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
