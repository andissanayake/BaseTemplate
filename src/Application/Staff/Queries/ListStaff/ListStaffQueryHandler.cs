using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Queries.ListStaff;

public class ListStaffQueryHandler : IRequestHandler<ListStaffQuery, List<StaffMemberDto>>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public ListStaffQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<StaffMemberDto>>> HandleAsync(ListStaffQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        // Get the tenant to identify the owner
        var tenant = await _context.Tenant.SingleAsync(t => t.Id == userProfile.TenantId, cancellationToken);

        // Get all users for the tenant (exclude soft deleted)
        var users = await _context.AppUser
            .Where(u => u.TenantId == userProfile.TenantId && !u.IsDeleted)
            .OrderByDescending(u => u.Created)
            .ToListAsync(cancellationToken);

        // Get all roles for all users in a single query (exclude soft deleted)
        var userIds = users.Select(u => u.Id).ToList();
        var roles = new List<UserRole>();
        if (userIds.Any())
        {
            roles = await _context.UserRole
                .Where(r => userIds.Contains(r.UserId) && !r.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        // Group roles by user Id for efficient lookup
        var rolesByUserId = roles.GroupBy(r => r.UserId).ToDictionary(g => g.Key, g => g.Select(r => r.Role).ToList());

        var result = new List<StaffMemberDto>();
        foreach (var user in users)
        {
            // Skip the actual tenant owner
            if (user.Id == tenant.OwnerId)
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
