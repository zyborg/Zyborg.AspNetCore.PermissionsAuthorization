// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorRouteData = Microsoft.AspNetCore.Components.RouteData;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public class PermissionsAuthorizationHandler : IAuthorizationHandler
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly PermissionsAuthorizationOptions _options;

    public PermissionsAuthorizationHandler(ILogger<PermissionsAuthorizationHandler> logger,
        IServiceScopeFactory scopeFactory,
        PermissionsAuthorizationOptions options)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _options = options;
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.PendingRequirements.Contains(HasPermissionsRequirement.Instance))
        {
            if (context.Resource is BlazorRouteData routeData)
            {
                using var scope = _scopeFactory.CreateScope();

                var user = context.User;
                var resolver = scope.ServiceProvider.GetRequiredService<IPermissionsResolver>();
                var perms = await resolver.GetPermissionsAsync(user);

                //_logger.LogInformation($"Acceptable Permissions combinations for Target:  [{routeData.PageType.FullName}]:");
                foreach (var aa in routeData.PageType.GetCustomAttributes<PermitAttribute>(true))
                {
                    //_logger.LogInformation($"  * Allow for: {string.Join(",", aa.RequiredPermissions)}");

                    if (aa.RequiredPermissions.All(rp => perms.Contains(rp)))
                    {
                        context.Succeed(HasPermissionsRequirement.Instance);
                        break;
                    }
                }
            }
            else
            {
                _logger.LogWarning("Authorization requires Permissions, but target resource cannot be resolved as Components Route Data");
            }
        }
    }
}
