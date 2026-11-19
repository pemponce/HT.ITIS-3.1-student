using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;


namespace Dotnet.Homeworks.Features.Behaviors;

[PipelineOrder(2)]
public class AdminValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAdminRequest, IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public AdminValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var messages = new List<string>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            messages.AddRange(result.Errors.Select(error => error.ErrorMessage));
        }

        if (messages.Count > 0)
            return ResultFactory.CreateFailure<TResponse>(string.Join("; ", messages.Distinct()));

        return await next();
    }
}
