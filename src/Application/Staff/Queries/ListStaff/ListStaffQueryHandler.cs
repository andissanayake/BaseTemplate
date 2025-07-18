namespace BaseTemplate.Application.Staff.Queries.ListStaff;

public class ListStaffQueryHandler : IRequestHandler<ListStaffQuery, List<StaffMemberDto>>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public ListStaffQueryHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<StaffMemberDto>>> HandleAsync(ListStaffQuery request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile?.TenantId == null)
        {
            return Result<List<StaffMemberDto>>.Forbidden("User is not associated with any tenant.");
        }

        var tenantId = userProfile.TenantId.Value;

        using var uow = _factory.Create();

        // Get the tenant to identify the owner
        var tenant = await uow.GetAsync<Tenant>(tenantId);
        if (tenant == null)
        {
            return Result<List<StaffMemberDto>>.NotFound($"Tenant with id {tenantId} not found.");
        }

        // Get all users for the tenant
        var users = await uow.QueryAsync<AppUser>(
            "SELECT * FROM app_user WHERE tenant_id = @TenantId ORDER BY created DESC",
            new { TenantId = tenantId });

        // Get all roles for all users in a single query
        var userIds = users.Select(u => u.Id).ToList();
        var roles = new List<UserRole>();

        if (userIds.Any())
        {
            var roleQuery = "SELECT * FROM user_role WHERE user_id = ANY(@UserIds)";
            roles = (await uow.QueryAsync<UserRole>(roleQuery, new { UserIds = userIds })).ToList();
        }

        // Group roles by user Id for efficient lookup
        var rolesByUserId = roles.GroupBy(r => r.UserId).ToDictionary(g => g.Key, g => g.Select(r => r.Role).ToList());

        var result = new List<StaffMemberDto>();

        foreach (var user in users)
        {
            // Skip the actual tenant owner
            if (user.SsoId == tenant.OwnerSsoId)
                continue;

            var dto = new StaffMemberDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = rolesByUserId.GetValueOrDefault(user.Id, new List<string>()),
                Created = user.Created,
                LastModified = user.LastModified
            };

            result.Add(dto);
        }

        return Result<List<StaffMemberDto>>.Success(result);
    }
} 