// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enables the Permissions-based Authorization support through registration
    /// of supporting dependencies.
    /// </summary>
    /// <remarks>
    /// Specifically, this routine will perform the following:
    /// <list type="bullet">
    /// <item>Registers an application-specific permissions resolver</item>
    /// <item>Registers an optional set of configuration options</item>
    /// <item>Enables the ASP.NET Core Authorization pipeline</item>
    /// <item>Adds a custom policy for targets that require permissions</item>
    /// <item>Registers a custom authorization handler</item>
    /// </list>
    /// </remarks>
    /// <param name="services"></param>
    /// <param name="optionsConfigure"></param>
    /// <returns></returns>
    public static IServiceCollection AddPermissionsAuthorization(this IServiceCollection services,
        Func<IServiceProvider, IPermissionsResolver> resolverFactory,
        Action<PermissionsAuthorizationOptions>? optionsConfigure = null)
    {
        services.AddPermissionsResolver(resolverFactory);
        services.AddPermissionsAuthorizationHandler(optionsConfigure);
        services.AddAuthorization(options => options.AddPermissionsAuthorizationPolicy());

        return services;
    }

    static IServiceCollection AddPermissionsResolver(this IServiceCollection services,
        Func<IServiceProvider, IPermissionsResolver> resolverFactory)
    {
        services.AddScoped<IPermissionsResolver>(services => resolverFactory(services));

        return services;
    }

    static IServiceCollection AddPermissionsAuthorizationHandler(this IServiceCollection services,
        Action<PermissionsAuthorizationOptions>? optionsConfigure = null)
    {
        services.AddSingleton<PermissionsAuthorizationOptions>(services =>
        {
            var options = new PermissionsAuthorizationOptions();
            if (optionsConfigure != null)
            {
                optionsConfigure(options);
            }
            return options;
        });
        services.AddSingleton<IAuthorizationHandler, PermissionsAuthorizationHandler>();

        return services;
    }

    static AuthorizationOptions AddPermissionsAuthorizationPolicy(this AuthorizationOptions options)
    {
        options.AddPolicy(PermitAttribute.HasPermissionsPolicy, policyBuilder =>
        {
            policyBuilder.Requirements.Add(HasPermissionsRequirement.Instance);
        });

        return options;
    }
}
