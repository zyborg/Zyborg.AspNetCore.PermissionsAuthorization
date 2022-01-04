// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using System.Security.Claims;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public interface IResourcePermittedResolver
{
    Task<bool> IsPermitted(Type resourceType, ClaimsPrincipal user);
}
