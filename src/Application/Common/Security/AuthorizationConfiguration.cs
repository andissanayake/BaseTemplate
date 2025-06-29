using Microsoft.AspNetCore.Authorization;

namespace BaseTemplate.Application.Common.Security;

public static class AuthorizationConfiguration
{
    public static void ConfigureAuthorization(AuthorizationOptions options)
    {
        // Configure CanPurge policy
        //options.AddPolicy(Policies.CanPurge, policy =>
        //    policy.RequireRole(Roles.Administrator));

        // Add more policies here as needed
        // Example:
        // options.AddPolicy("CanManageUsers", policy =>
        //     policy.RequireRole(Roles.Administrator, Roles.Manager));

        // options.AddPolicy("CanViewReports", policy =>
        //     policy.RequireRole(Roles.Administrator, Roles.Manager, Roles.Viewer));
    }
}
