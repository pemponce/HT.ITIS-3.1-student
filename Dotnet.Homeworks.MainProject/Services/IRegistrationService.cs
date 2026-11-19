using Dotnet.Homeworks.MainProject.Dto;

namespace Dotnet.Homeworks.MainProject.Services;

public interface IRegistrationService
{
    Task RegisterAsync(RegisterUserDto userDto);
}