namespace Dotnet.Homeworks.Features.Users.Abstractions;

/// <summary>
/// Подписчик на событие "пользователь создан". Реализации живут во внешних слоях (например, в MainProject
/// отправляют приветственное письмо), поэтому Features не зависит от них (Dependency Inversion).
/// </summary>
public interface IUserCreatedListener
{
    Task OnUserCreatedAsync(Guid guid, string name, string email, CancellationToken cancellationToken);
}
