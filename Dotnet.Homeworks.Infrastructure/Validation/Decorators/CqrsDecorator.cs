using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;


namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

/// <summary>
/// Начало цепочки декораторов. Все хэндлеры фичи Users наследуются от него.
/// Цепочка вызовов: CqrsDecorator -> PermissionDecorator -> ValidationDecorator -> хэндлер (HandleCore).
/// </summary>
public abstract class CqrsDecorator<TRequest, TResponse> : PermissionDecorator<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    protected CqrsDecorator(IPermissionCheck permissionCheck, IEnumerable<IValidator<TRequest>> validators)
        : base(permissionCheck, validators)
    {
    }

    public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        => await base.Handle(request, cancellationToken);
}
