// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using System.Reflection;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public class PermissionsHelper
{
    private readonly ILogger _logger;
    private readonly PermissionsAuthorizationOptions _options;
    private readonly PermissionsContext _context;
    private readonly IResourcePermittedResolver _resolver;

    public PermissionsHelper(ILogger<PermissionsHelper> logger,
        PermissionsAuthorizationOptions options,
        PermissionsContext context,
        IResourcePermittedResolver resolver)
    {
        _logger = logger;
        _options = options;
        _context = context;
        _resolver = resolver;
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
            var userPerms = await _context.GetPermissionsAsync(user);
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

    private async Task<bool> ResolveIsPermittedAsync(Type resourceType, ClaimsPrincipal user)
    {
        return await _resolver.IsPermitted(_context, resourceType, user);
    }

}
