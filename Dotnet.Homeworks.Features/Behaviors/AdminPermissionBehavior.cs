using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;


namespace Dotnet.Homeworks.Features.Behaviors;

[PipelineOrder(1)]
public class AdminPermissionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAdminRequest, IRequest<TResponse>
    where TResponse : Result
{
    private readonly IPermissionCheck _permissionCheck;

    public AdminPermissionBehavior(IPermissionCheck permissionCheck)
    {
        _permissionCheck = permissionCheck;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var results = await _permissionCheck.CheckPermissionAsync(request);

        var failures = results.Where(result => result.IsFailure).ToList();
        if (failures.Count > 0)
            return ResultFactory.CreateFailure<TResponse>(string.Join("; ", failures.Select(f => f.Error)));

        return await next();
    }
}
