using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.Guid).NotEmpty().WithMessage("Не указан идентификатор пользователя");
        RuleFor(command => command.User.Email).IsValidEmail();
        RuleFor(command => command.User.Name).NotEmpty().WithMessage("Имя не должно быть пустым");
    }
}
