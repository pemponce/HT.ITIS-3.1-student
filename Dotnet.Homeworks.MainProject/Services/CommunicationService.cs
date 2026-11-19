using Dotnet.Homeworks.Shared.MessagingContracts.Email;
using MassTransit;

namespace Dotnet.Homeworks.MainProject.Services;

public class CommunicationService : ICommunicationService
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CommunicationService(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task SendEmailAsync(SendEmail message)
    {
        return _publishEndpoint.Publish(message);
    }
}