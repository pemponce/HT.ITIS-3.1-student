using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Queries.GetUser;

public class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(query => query.Guid).NotEmpty().WithMessage("Не указан идентификатор пользователя");
    }
}
