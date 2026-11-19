using FluentValidation;

namespace Dotnet.Homeworks.Features.UserManagement.Commands.DeleteUserByAdmin;

public class DeleteUserByAdminCommandValidator : AbstractValidator<DeleteUserByAdminCommand>
{
    public DeleteUserByAdminCommandValidator()
    {
        RuleFor(command => command.Guid).NotEmpty().WithMessage("Не указан идентификатор пользователя");
    }
}
