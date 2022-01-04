// Zyborg Permissions=based Authorization for ASP.NET Core.
// Copyright (C) Zyborg.

using Microsoft.AspNetCore.Authorization;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PermitAttribute : AuthorizeAttribute
{

    public PermitAttribute(params string[] requiredPermissions)
        : base(policy: UserIsPermittedRequirement.UserIsPermittedPolicy)
    {
        RequiredPermissions = requiredPermissions;
    }

    public IEnumerable<string> RequiredPermissions { get; }
}
