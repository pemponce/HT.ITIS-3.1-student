using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Users.PermissionChecks;

/// <summary>
/// Обычный пользователь может выполнять операции только над своим аккаунтом:
/// ClaimTypes.NameIdentifier из HttpContext должен совпадать с Guid в запросе.
/// </summary>
public class ClientRequestPermissionCheck<TRequest> : IRequestPermissionCheck<TRequest>
    where TRequest : IClientRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientRequestPermissionCheck(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<PermissionResult> CheckPermissionAsync(TRequest request)
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims;
        var nameIdentifier = claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        var isAllowed = Guid.TryParse(nameIdentifier, out var userGuid) && userGuid == request.Guid;

        return Task.FromResult(isAllowed
            ? new PermissionResult(true)
            : new PermissionResult(false, "Нет доступа: можно работать только со своим аккаунтом"));
    }
}
