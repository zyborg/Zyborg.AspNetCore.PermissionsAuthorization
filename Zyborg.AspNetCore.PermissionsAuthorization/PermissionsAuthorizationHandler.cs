// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        _logger.LogTrace("CREATED Perms Authz Handler");
    }

    public async Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.PendingRequirements.Contains(UserIsPermittedRequirement.Instance))
        {
            if (context.Resource is BlazorRouteData routeData)
            {
                using var scope = _scopeFactory.CreateScope();
                var pctx = scope.ServiceProvider.GetRequiredService<PermissionsContext>();
                if (await pctx.IsPermittedAsync(routeData.PageType, context.User))
                {
                    context.Succeed(UserIsPermittedRequirement.Instance);
                }
            }
            else
            {
                _logger.LogWarning("Authorization requires Permissions, but target resource cannot be resolved as Components Route Data");
            }
        }
    }
}
