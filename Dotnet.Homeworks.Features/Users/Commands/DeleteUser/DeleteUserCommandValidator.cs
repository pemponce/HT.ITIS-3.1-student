using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.DeleteUser;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.Guid).NotEmpty().WithMessage("Не указан идентификатор пользователя");
    }
}
