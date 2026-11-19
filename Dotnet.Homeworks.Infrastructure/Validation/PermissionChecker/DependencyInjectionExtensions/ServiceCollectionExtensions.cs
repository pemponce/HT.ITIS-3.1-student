using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        Assembly assembly
    )
    {
        serviceCollection.AddPermissionChecks(new[] { assembly });
    }

    /// <summary>
    /// Регистрирует <see cref="IPermissionCheck"/> и все реализации <see cref="IRequestPermissionCheck{TRequest}"/>
    /// из переданных сборок. Поддерживаются как обычные классы (для одного запроса), так и открытые generic-классы
    /// вида <c>Check&lt;TRequest&gt; : IRequestPermissionCheck&lt;TRequest&gt; where TRequest : IClientRequest</c>.
    /// </summary>
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        Assembly[] assemblies
    )
    {
        serviceCollection.AddScoped<IPermissionCheck, PermissionCheck>();

        foreach (var type in assemblies.SelectMany(GetLoadableTypes))
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            foreach (var @interface in type.GetInterfaces())
            {
                if (!@interface.IsGenericType ||
                    @interface.GetGenericTypeDefinition() != typeof(IRequestPermissionCheck<>))
                    continue;

                if (type.IsGenericTypeDefinition)
                    serviceCollection.AddScoped(typeof(IRequestPermissionCheck<>), type);
                else
                    serviceCollection.AddScoped(@interface, type);
            }
        }
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
