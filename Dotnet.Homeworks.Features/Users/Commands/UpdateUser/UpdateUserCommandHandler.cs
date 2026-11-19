using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler
    : CqrsDecorator<UpdateUserCommand, Result>, ICommandHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPermissionCheck permissionCheck,
        IEnumerable<IValidator<UpdateUserCommand>> validators)
        : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task<Result> HandleCore(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByGuidAsync(request.Guid, cancellationToken);
        if (user is null)
            return new Result(false, "Пользователь не найден");

        var users = await _userRepository.GetUsersAsync(cancellationToken);
        if (users.Any(other => other.Email == request.User.Email && other.Id != request.Guid))
            return new Result(false, "Пользователь с такой почтой уже существует");

        user.Name = request.User.Name;
        user.Email = request.User.Email;

        await _userRepository.UpdateUserAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new Result(true);
    }
}
