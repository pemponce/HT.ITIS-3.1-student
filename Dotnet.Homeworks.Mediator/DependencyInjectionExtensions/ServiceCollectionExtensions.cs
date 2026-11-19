using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services,
        params Assembly[] handlersAssemblies)
    {
        foreach (var handlerType in handlersAssemblies.SelectMany(GetLoadableTypes))
        {
            if (!handlerType.IsClass || handlerType.IsAbstract || handlerType.IsGenericTypeDefinition)
                continue;

            foreach (var @interface in handlerType.GetInterfaces())
            {
                if (!@interface.IsGenericType)
                    continue;

                var definition = @interface.GetGenericTypeDefinition();
                if (definition == typeof(IRequestHandler<,>) || definition == typeof(IRequestHandler<>))
                    services.AddTransient(@interface, handlerType);
            }
        }

        services.AddScoped<IMediator, DefaultMediator>();

        return services;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
    }
}
