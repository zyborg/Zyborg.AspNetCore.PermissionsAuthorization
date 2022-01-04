// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using System.Security.Claims;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public class PermissionsAuthorizationOptions
{
    /// <summary>
    /// When enabled, will cache the resolved permissions within the context of
    /// the <c>HttpContext</c> <c>Items</c> map.  This defaults to <c>true</c>.
    /// </summary>
    /// <remarks>
    /// Enabling this setting can potentially improve performance and efficiency
    /// of resources for particularly complex and resource-itensive permissions
    /// resolution such as querying from a database or making web service calls.
    /// <para>
    /// Resolved permissions are only resolved and cached for the associated HttpContext
    /// request scope which may have different effects depending on the ASP.NET
    /// technology in use.  For example, for a typical ASP.NET Core MVC or Web API
    /// type of application, this will still require resolving the permissions
    /// once per request/response invocation.
    /// </para><para>
    /// However in the context of a technology like Blazor Server, this
    /// request/response cycle can last for the duration of a user's
    /// <i>effective</i> session or long-running interaction with the
    /// main application.  In this particular case, enabling this setting
    /// will have vast performance benefits for expensive resolvers.
    /// </para>
    /// </remarks>
    public bool CachePermissionsPerHttpContext { get; set; } = true;
}
