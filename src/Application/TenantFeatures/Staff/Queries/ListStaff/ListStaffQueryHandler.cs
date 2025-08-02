using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Staff.Queries.ListStaff;

public class ListStaffQueryHandler(IAppDbContext context, IUserProfileService userProfileService) : IRequestHandler<ListStaffQuery, List<StaffMemberDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<Result<List<StaffMemberDto>>> HandleAsync(ListStaffQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenant
                .AsNoTracking()
                .SingleAsync(t =>
                    t.Id == _userProfileService.UserProfile.TenantId,
                    cancellationToken);

        // Get all users for the tenant (exclude soft deleted)
        var users = await _context.AppUser
            .AsNoTracking()
            .Where(u =>
                u.TenantId == _userProfileService.UserProfile.TenantId &&
                u.Id != tenant.OwnerId &&
                u.Id != _userProfileService.UserProfile.Id)
            .OrderByDescending(u => u.Created)
            .ToListAsync(cancellationToken);


        // Get all roles for all users in a single query (exclude soft deleted)
        var userIds = users.Select(u => u.Id).ToList();
        var roles = new List<UserRole>();
        if (userIds.Count != 0)
        {
            roles = await _context.UserRole.AsNoTracking()
                .Where(r => userIds.Contains(r.UserId))
                .ToListAsync(cancellationToken);
        }

        // Group roles by user Id for efficient lookup
        var rolesByUserId = roles.GroupBy(r => r.UserId).ToDictionary(g => g.Key, g => g.Select(r => r.Role).ToList());

        var result = new List<StaffMemberDto>();
        foreach (var user in users)
        {
            var dto = new StaffMemberDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = rolesByUserId.GetValueOrDefault(user.Id, []),
                Created = user.Created,
                LastModified = user.LastModified
            };
            result.Add(dto);
        }
        return Result<List<StaffMemberDto>>.Success(result);
    }
}
