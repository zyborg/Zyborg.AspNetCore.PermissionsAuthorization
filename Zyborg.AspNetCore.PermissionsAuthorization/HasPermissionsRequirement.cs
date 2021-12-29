// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using Microsoft.AspNetCore.Authorization;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public sealed class HasPermissionsRequirement : IAuthorizationRequirement
{
    public static readonly HasPermissionsRequirement Instance = new();

    private HasPermissionsRequirement()
    { }
}
