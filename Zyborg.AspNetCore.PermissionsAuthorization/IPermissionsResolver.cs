// Sontiq XFER.
// Copyright (C) Sontiq.

using System.Security.Claims;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public interface IPermissionsResolver
{
    Task<ISet<string>> GetPermissionsAsync(ClaimsPrincipal user);
}
