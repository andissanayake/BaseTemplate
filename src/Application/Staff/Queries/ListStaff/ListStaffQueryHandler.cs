namespace BaseTemplate.Application.Staff.Queries.ListStaff;

public class ListStaffQueryHandler : IRequestHandler<ListStaffQuery, List<StaffMemberDto>>
{
    private readonly IUnitOfWorkFactory _factory;

    public ListStaffQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<List<StaffMemberDto>>> HandleAsync(ListStaffQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Get all users for the tenant
        var users = await uow.QueryAsync<AppUser>(
            "SELECT * FROM app_user WHERE tenant_id = @TenantId ORDER BY created DESC",
            new { request.TenantId });

        // Get all roles for all users in a single query
        var userSsoIds = users.Select(u => u.SsoId).ToList();
        var roles = new List<UserRole>();

        if (userSsoIds.Any())
        {
            var roleQuery = "SELECT * FROM user_role WHERE user_sso_id = ANY(@UserSsoIds)";
            roles = (await uow.QueryAsync<UserRole>(roleQuery, new { UserSsoIds = userSsoIds })).ToList();
        }

        // Group roles by user SsoId for efficient lookup
        var rolesByUserSsoId = roles.GroupBy(r => r.UserSsoId).ToDictionary(g => g.Key, g => g.Select(r => r.Role).ToList());

        var result = new List<StaffMemberDto>();

        foreach (var user in users)
        {
            var dto = new StaffMemberDto
            {
                SsoId = user.SsoId,
                Name = user.Name,
                Email = user.Email,
                Roles = rolesByUserSsoId.GetValueOrDefault(user.SsoId, new List<string>()),
                Created = user.Created,
                LastModified = user.LastModified
            };

            result.Add(dto);
        }

        return Result<List<StaffMemberDto>>.Success(result);
    }
} 