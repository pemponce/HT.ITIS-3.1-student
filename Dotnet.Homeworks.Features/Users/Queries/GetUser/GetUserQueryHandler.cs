using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Queries;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Queries.GetUser;

public sealed class GetUserQueryHandler
    : CqrsDecorator<GetUserQuery, Result<GetUserDto>>, IQueryHandler<GetUserQuery, GetUserDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(
        IUserRepository userRepository,
        IPermissionCheck permissionCheck,
        IEnumerable<IValidator<GetUserQuery>> validators)
        : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
    }

    protected override async Task<Result<GetUserDto>> HandleCore(GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);
        if (user is null)
            return new Result<GetUserDto>(null, false, "Пользователь не найден");

        return new Result<GetUserDto>(new GetUserDto(user.Id, user.Name, user.Email), true);
    }
}
