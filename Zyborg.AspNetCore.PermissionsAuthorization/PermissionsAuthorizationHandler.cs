// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using Microsoft.AspNetCore.Authorization;
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
                var helper = scope.ServiceProvider.GetRequiredService<PermissionsHelper>();
                if (await helper.IsPermittedAsync(routeData.PageType, context.User))
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
