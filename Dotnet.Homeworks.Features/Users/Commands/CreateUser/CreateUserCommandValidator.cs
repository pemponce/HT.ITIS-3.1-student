using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.Email).IsValidEmail();
        RuleFor(command => command.Name).NotEmpty().WithMessage("Имя не должно быть пустым");
    }
}
