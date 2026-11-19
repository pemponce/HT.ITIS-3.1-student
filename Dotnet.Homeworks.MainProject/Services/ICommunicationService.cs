using Dotnet.Homeworks.Shared.MessagingContracts.Email;

namespace Dotnet.Homeworks.MainProject.Services;

public interface ICommunicationService
{
    Task SendEmailAsync(SendEmail message);
}