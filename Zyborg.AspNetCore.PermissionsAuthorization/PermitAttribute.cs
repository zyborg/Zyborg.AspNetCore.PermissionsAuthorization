﻿// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using Microsoft.AspNetCore.Authorization;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PermitAttribute : AuthorizeAttribute
{
    internal const string HasPermissionsPolicy = $"__{nameof(HasPermissionsPolicy)}__";

    public PermitAttribute(params string[] requiredPermissions)
        : base(policy: HasPermissionsPolicy)
    {
        RequiredPermissions = requiredPermissions;
    }

    public IEnumerable<string> RequiredPermissions { get; }
}
