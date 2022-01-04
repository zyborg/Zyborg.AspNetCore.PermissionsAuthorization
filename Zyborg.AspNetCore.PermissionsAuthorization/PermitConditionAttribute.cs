// Sontiq XFER.
// Copyright (C) Sontiq.

using Microsoft.AspNetCore.Authorization;

namespace Zyborg.AspNetCore.PermissionsAuthorization;

/// <summary>
/// This marker attribute is used to indicate that permission to
/// access a resource should be resolved using a resource permit
/// resolver.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PermitConditionAttribute : AuthorizeAttribute
{
    public PermitConditionAttribute()
        : base(UserIsPermittedRequirement.UserIsPermittedPolicy)
    { }
}
