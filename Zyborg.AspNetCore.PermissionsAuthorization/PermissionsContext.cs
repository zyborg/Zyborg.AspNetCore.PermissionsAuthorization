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

    public async Task<bool> HasPermissionsAsync(ClaimsPrincipal user, params string[] permissions)
    {
        var perms = await GetPermissionsAsync(user);
        return permissions.All(p => perms.Contains(p));
    }

    public async Task<ISet<string>> GetPermissionsAsync(ClaimsPrincipal user)
    {
        if (user == _httpAccessor.HttpContext?.User)
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
}
