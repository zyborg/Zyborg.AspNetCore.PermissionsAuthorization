// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using System.Security.Claims;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public interface IUserPermissionsResolver
{
    Task<ISet<string>> GetPermissionsAsync(ClaimsPrincipal user);
}
