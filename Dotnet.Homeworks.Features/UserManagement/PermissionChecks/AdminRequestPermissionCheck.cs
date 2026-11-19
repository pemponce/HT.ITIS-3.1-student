using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.Enums;
namespace Dotnet.Homeworks.Features.UserManagement.PermissionChecks;

/// <summary>Операции управления пользователями доступны только администраторам (ClaimTypes.Role).</summary>
public class AdminRequestPermissionCheck<TRequest> : IRequestPermissionCheck<TRequest>
    where TRequest : IAdminRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminRequestPermissionCheck(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<PermissionResult> CheckPermissionAsync(TRequest request)
    {
        var claims = _httpContextAccessor.HttpContext?.User?.Claims;
        var isAdmin = claims?.Any(claim =>
            claim.Type == ClaimTypes.Role && claim.Value == Roles.Admin.ToString()) ?? false;

        return Task.FromResult(isAdmin
            ? new PermissionResult(true)
            : new PermissionResult(false, "Нет доступа: операция доступна только администраторам"));
    }
}
