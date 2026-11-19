using Dotnet.Homeworks.MainProject.Dto;
using Dotnet.Homeworks.Shared.MessagingContracts.Email;

namespace Dotnet.Homeworks.MainProject.Services;

public class RegistrationService : IRegistrationService
{
    private readonly ICommunicationService _communicationService;

    public RegistrationService(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    public Task RegisterAsync(RegisterUserDto userDto)
    {
        var message = new SendEmail(
            userDto.Name,
            userDto.Email,
            "Добро пожаловать!",
            $"Здравствуйте, {userDto.Name}! Спасибо за регистрацию.");

        return _communicationService.SendEmailAsync(message);
    }
}