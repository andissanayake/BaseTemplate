using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Queries.ListStaff;

public class ListStaffQueryHandler : IRequestHandler<ListStaffQuery, List<StaffMemberDto>>
{
    private readonly IAppDbContext _context;

    public ListStaffQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<StaffMemberDto>>> HandleAsync(ListStaffQuery request, CancellationToken cancellationToken)
    {
        // Get all users for the tenant (exclude soft deleted)
        var users = await _context.AppUser
            .OrderByDescending(u => u.Created)
            .ToListAsync(cancellationToken);

        // Get all roles for all users in a single query (exclude soft deleted)
        var userIds = users.Select(u => u.Id).ToList();
        var roles = new List<UserRole>();
        if (userIds.Any())
        {
            roles = await _context.UserRole
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
                Roles = rolesByUserId.GetValueOrDefault(user.Id, new List<string>()),
                Created = user.Created,
                LastModified = user.LastModified
            };
            result.Add(dto);
        }
        return Result<List<StaffMemberDto>>.Success(result);
    }
}
