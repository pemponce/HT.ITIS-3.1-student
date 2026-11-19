using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;
using FluentValidation.Results;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public abstract class ValidationDecorator<TRequest, TResponse> : RequestHandlerDecoratorBase<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    protected ValidationDecorator(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public override async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            errors.AddRange(result.Errors);
        }

        if (errors.Count > 0)
        {
            var message = string.Join("; ", errors.Select(error => error.ErrorMessage).Distinct());
            return ResultFactory.CreateFailure<TResponse>(message);
        }

        return await base.Handle(request, cancellationToken);
    }
}
