using FluentValidation;

namespace Dotnet.Homeworks.Features.Users;

public static class EmailValidationExtensions
{
    private const string EmailPattern =
        @"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?(?:\.[A-Za-z0-9](?:[A-Za-z0-9\-]*[A-Za-z0-9])?)*\.[A-Za-z]{2,}$";

    public static IRuleBuilderOptions<T, string> IsValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Почта не должна быть пустой")
            .Matches(EmailPattern).WithMessage("Некорректная почта");
    }
}
