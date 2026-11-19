using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public abstract class PermissionDecorator<TRequest, TResponse> : ValidationDecorator<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IPermissionCheck _permissionCheck;

    protected PermissionDecorator(IPermissionCheck permissionCheck, IEnumerable<IValidator<TRequest>> validators)
        : base(validators)
    {
        _permissionCheck = permissionCheck;
    }

    public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var results = await _permissionCheck.CheckPermissionAsync(request);

        var failures = results.Where(result => result.IsFailure).ToList();
        if (failures.Count > 0)
        {
            var message = string.Join("; ", failures.Select(failure => failure.Error));
            return ResultFactory.CreateFailure<TResponse>(message);
        }

        return await base.Handle(request, cancellationToken);
    }
}
