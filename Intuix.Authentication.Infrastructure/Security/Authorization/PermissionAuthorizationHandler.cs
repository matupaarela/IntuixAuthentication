using Microsoft.AspNetCore.Authorization;

namespace Intuix.Authentication.Infrastructure.Security.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permissions = context.User.FindAll("perm")
            .Select(x => x.Value);

        var required = requirement.Permission.Split('|');

        if (required.Any(p => permissions.Contains(p)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
