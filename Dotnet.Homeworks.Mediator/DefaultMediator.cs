using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Homeworks.Mediator;

public sealed class DefaultMediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultMediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var wrapper = RequestHandlerWrapper.GetOrCreate(request.GetType())
                      ?? throw new InvalidOperationException(
                          $"Тип {request.GetType().FullName} не реализует IRequest<TResponse>");

        var response = await wrapper.Handle(request, _serviceProvider, cancellationToken);
        return (TResponse)response!;
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = _serviceProvider.GetService<IRequestHandler<TRequest>>()
                      ?? throw new InvalidOperationException(
                          $"Не найден обработчик для запроса {typeof(TRequest).FullName}");

        await handler.Handle(request, cancellationToken);
    }

    public async Task<dynamic?> Send(dynamic request, CancellationToken cancellationToken = default)
    {
        object? untypedRequest = request;
        if (untypedRequest is null)
            return null;

        var requestType = untypedRequest.GetType();

        var wrapper = RequestHandlerWrapper.GetOrCreate(requestType);
        if (wrapper is not null)
        {
            return wrapper.HasHandler(_serviceProvider)
                ? await wrapper.Handle(untypedRequest, _serviceProvider, cancellationToken)
                : null;
        }

        if (untypedRequest is IRequest)
        {
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
            var handler = _serviceProvider.GetService(handlerType);
            if (handler is null)
                return null;

            var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest>.Handle))!;
            await (Task)handleMethod.Invoke(handler, new[] { untypedRequest, cancellationToken })!;
            return null;
        }

        return null;
    }
}
