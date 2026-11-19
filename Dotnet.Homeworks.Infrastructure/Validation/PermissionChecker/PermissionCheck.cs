using Dotnet.Homeworks.Infrastructure.Utils;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

public class PermissionCheck : IPermissionCheck
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Находит все проверки, подходящие для запроса (например, по NameIdentifier для IClientRequest
    /// или по роли для IAdminRequest), и выполняет их. Если подходящих проверок нет, результат пустой (доступ открыт).
    /// </summary>
    public async Task<IEnumerable<PermissionResult>> CheckPermissionAsync<TRequest>(TRequest request)
    {
        var checks = _serviceProvider.GetServices<IRequestPermissionCheck<TRequest>>();

        var results = new List<PermissionResult>();
        foreach (var check in checks)
            results.Add(await check.CheckPermissionAsync(request));

        return results;
    }
}
