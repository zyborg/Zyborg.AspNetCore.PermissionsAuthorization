// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using Microsoft.AspNetCore.Authorization;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

public sealed class UserIsPermittedRequirement : IAuthorizationRequirement
{
    public static readonly UserIsPermittedRequirement Instance = new();

    internal const string UserIsPermittedPolicy = $"__{nameof(UserIsPermittedPolicy)}__";

    private UserIsPermittedRequirement()
    { }
}
