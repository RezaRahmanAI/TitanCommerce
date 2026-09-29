using Microsoft.AspNetCore.Authorization;

namespace TitanCommerce.Application.Common.Security;

public sealed class HasPermissionAtribute : AuthorizeAttribute
{
    public HasPermissionAtribute(string permission) : base(policy: permission)
    {

    }
}