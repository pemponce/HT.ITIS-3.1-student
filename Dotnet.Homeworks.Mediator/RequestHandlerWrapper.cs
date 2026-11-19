using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator;

internal abstract class RequestHandlerWrapper
{
    private static readonly ConcurrentDictionary<Type, RequestHandlerWrapper?> Cache = new();
    private static readonly ConcurrentDictionary<Type, int> OrderCache = new();

    public static RequestHandlerWrapper? GetOrCreate(Type requestType)
    {
        return Cache.GetOrAdd(requestType, static type =>
        {
            var responseType = GetResponseType(type);
            if (responseType is null)
                return null;

            var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(type, responseType);
            return (RequestHandlerWrapper)Activator.CreateInstance(wrapperType)!;
        });
    }

    public static Type? GetResponseType(Type requestType)
    {
        return requestType
            .GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
            ?.GetGenericArguments()[0];
    }

    protected static int GetOrder(Type behaviorType)
    {
        return OrderCache.GetOrAdd(behaviorType,
            static type => type.GetCustomAttribute<PipelineOrderAttribute>()?.Order ?? 0);
    }

    public abstract bool HasHandler(IServiceProvider provider);

    public abstract Task<object?> Handle(object request, IServiceProvider provider,
        CancellationToken cancellationToken);
}

internal sealed class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerWrapper
    where TRequest : IRequest<TResponse>
{
    public override bool HasHandler(IServiceProvider provider)
        => provider.GetService<IRequestHandler<TRequest, TResponse>>() is not null;

    public override async Task<object?> Handle(object request, IServiceProvider provider,
        CancellationToken cancellationToken)
    {
        var handler = provider.GetService<IRequestHandler<TRequest, TResponse>>()
                      ?? throw new InvalidOperationException(
                          $"Не найден обработчик для запроса {typeof(TRequest).FullName}");

        var typedRequest = (TRequest)request;

        var behaviors = provider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .OrderBy(behavior => GetOrder(behavior.GetType()))
            .ToList();

        RequestHandlerDelegate<TResponse> next = () => handler.Handle(typedRequest, cancellationToken);

        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var current = next;
            next = () => behavior.Handle(typedRequest, current, cancellationToken);
        }

        return await next();
    }
}
