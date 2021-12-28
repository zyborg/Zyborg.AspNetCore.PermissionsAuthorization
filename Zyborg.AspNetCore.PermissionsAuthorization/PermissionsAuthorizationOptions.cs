// Sontiq XFER.
// Copyright (C) Sontiq.

using System.Security.Claims;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public class PermissionsAuthorizationOptions
{
    public Func<IServiceProvider, ClaimsPrincipal, Task<HashSet<string>>>? PermissionsResolver { get; set;}
}
