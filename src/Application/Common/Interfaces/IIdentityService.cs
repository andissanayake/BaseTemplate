﻿namespace BaseTemplate.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<IEnumerable<string>> GetRolesAsync(string userId);
}
