// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public class PermissionsContext
{
    private readonly ILogger _logger;
    private readonly IHttpContextAccessor _httpAccessor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PermissionsAuthorizationOptions _options;

    public PermissionsContext(ILogger<PermissionsAuthorizationHandler> logger,
        IHttpContextAccessor httpAccessor,
        IServiceScopeFactory scopeFactory,
        PermissionsAuthorizationOptions options)
    {
        _logger = logger;
        _httpAccessor = httpAccessor;
        _scopeFactory = scopeFactory;
        _options = options;

        _logger.LogTrace("CREATED Perms Context");
    }

    public Task<bool> IsPermittedAsync<TResource>(ClaimsPrincipal user) =>
        IsPermittedAsync(typeof(TResource), user);

    public async Task<bool> IsPermittedAsync(Type resourceType, ClaimsPrincipal user)
    {
        if (resourceType.GetCustomAttribute<PermitConditionAttribute>() != null)
        {
            return await ResolveIsPermittedAsync(resourceType, user);
        }

        var permitAttributes = resourceType.GetCustomAttributes<PermitAttribute>()?.ToArray();
        if (permitAttributes != null && permitAttributes.Length > 0)
        {
            var userPerms = await GetPermissionsAsync(user);
            foreach (var aa in permitAttributes)
            {
                if (aa.RequiredPermissions.All(rp => userPerms.Contains(rp)))
                {
                    return true;
                }
            }

            return false;
        }

        return true;
    }

    public async Task<ISet<string>> GetPermissionsAsync(ClaimsPrincipal? user)
    {
        if (user == null || user == _httpAccessor.HttpContext?.User)
        {
            if (_options.CachePermissionsPerHttpContext)
            {
                if (_httpAccessor.HttpContext?.Items is IDictionary<object, object> httpItems)
                {
                    // We try to use the current HTTP Context request items as
                    // a cache to minimize invoking resolution of permissions
                    if (!httpItems.TryGetValue(nameof(IUserPermissionsResolver), out var permsItem)
                        || permsItem is not ISet<string> perms)
                    {
                        // Fallback to full resolution
                        _logger.LogInformation("Permissions not yet cached for this request");
                        perms = await ResolvePermissionsAsync(user);
                        if (httpItems != null)
                        {
                            httpItems[nameof(IUserPermissionsResolver)] = perms;
                        }
                    }
                    return perms;
                }
                else
                {
                    _logger.LogWarning("Caching enabled but HttpContext Items are not available");
                }
            }
        }

        // Either cache is disabled or the HttpContext
        // Items dictionary is not available or the target
        // user is not the current context user so resolve
        return await ResolvePermissionsAsync(user);
    }

    private async Task<ISet<string>> ResolvePermissionsAsync(ClaimsPrincipal user)
    {
        _logger.LogInformation("Resolving permissions for target user");
        using var scope = _scopeFactory.CreateScope();

        var resolver = scope.ServiceProvider.GetRequiredService<IUserPermissionsResolver>();
        return await resolver.GetPermissionsAsync(user);
    }

    private async Task<bool> ResolveIsPermittedAsync(Type resourceType, ClaimsPrincipal user)
    {
        _logger.LogInformation("Resolving is permitted for target user and resource: " + resourceType.Name);
        using var scope = _scopeFactory.CreateScope();
        var resolver = scope.ServiceProvider.GetRequiredService<IResourcePermittedResolver>();
        return await resolver.IsPermitted(resourceType, user);
    }
}
