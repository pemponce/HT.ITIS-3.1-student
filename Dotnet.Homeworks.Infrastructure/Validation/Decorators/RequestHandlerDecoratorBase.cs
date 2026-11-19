using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public abstract class RequestHandlerDecoratorBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public virtual Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        => HandleCore(request, cancellationToken);

    protected abstract Task<TResponse> HandleCore(TRequest request, CancellationToken cancellationToken);
}
