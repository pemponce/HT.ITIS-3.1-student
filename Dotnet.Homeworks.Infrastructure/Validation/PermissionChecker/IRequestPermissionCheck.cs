using Dotnet.Homeworks.Infrastructure.Utils;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

/// <summary>
/// Проверка разрешения для запроса определённого вида. Реализации хранятся вместе с фичами,
/// а <see cref="PermissionCheck"/> работает только с этой абстракцией.
/// </summary>
public interface IRequestPermissionCheck<in TRequest>
{
    Task<PermissionResult> CheckPermissionAsync(TRequest request);
}
