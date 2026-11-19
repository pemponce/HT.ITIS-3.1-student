using Dotnet.Homeworks.Domain.Abstractions.Repositories;
using Dotnet.Homeworks.Domain.Entities;
using Dotnet.Homeworks.Features.Users.Abstractions;
using Dotnet.Homeworks.Infrastructure.Cqrs.Commands;
using Dotnet.Homeworks.Infrastructure.UnitOfWork;
using Dotnet.Homeworks.Infrastructure.Validation.Decorators;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Features.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler
    : CqrsDecorator<CreateUserCommand, Result<CreateUserDto>>, ICommandHandler<CreateUserCommand, CreateUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnumerable<IUserCreatedListener> _listeners;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IEnumerable<IUserCreatedListener> listeners,
        IPermissionCheck permissionCheck,
        IEnumerable<IValidator<CreateUserCommand>> validators)
        : base(permissionCheck, validators)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _listeners = listeners;
    }

    protected override async Task<Result<CreateUserDto>> HandleCore(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersAsync(cancellationToken);
        if (users.Any(user => user.Email == request.Email))
            return new Result<CreateUserDto>(null, false, "Пользователь с такой почтой уже существует");

        var guid = await _userRepository.InsertUserAsync(
            new User { Name = request.Name, Email = request.Email }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // регистрация пользователя (например, отправка письма) выполняется подписчиками из внешних слоёв
        foreach (var listener in _listeners)
            await listener.OnUserCreatedAsync(guid, request.Name, request.Email, cancellationToken);

        return new Result<CreateUserDto>(new CreateUserDto(guid), true);
    }
}
