using Dotnet.Homeworks.Shared.MessagingContracts.Email;
using MassTransit;

namespace Dotnet.Homeworks.MainProject.Services;

public class CommunicationService : ICommunicationService
{
    private readonly IBus _bus;

    public CommunicationService(IBus bus)
    {
        _bus = bus;
    }

    public Task SendEmailAsync(SendEmail sendEmailDto)
    {
        return _bus.Publish(sendEmailDto);
    }
}
