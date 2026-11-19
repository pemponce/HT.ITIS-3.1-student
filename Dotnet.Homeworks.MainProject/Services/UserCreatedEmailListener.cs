using Dotnet.Homeworks.Features.Users.Abstractions;
using Dotnet.Homeworks.MainProject.Dto;

namespace Dotnet.Homeworks.MainProject.Services;

public class UserCreatedEmailListener : IUserCreatedListener
{
    private readonly IRegistrationService _registrationService;

    public UserCreatedEmailListener(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    public Task OnUserCreatedAsync(Guid guid, string name, string email, CancellationToken cancellationToken)
    {
        return _registrationService.RegisterAsync(new RegisterUserDto(name, email));
    }
}
