using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Queries.GetStaffMember;
[Authorize(Roles = Roles.StaffManager)]
public record GetStaffMemberQuery(int StaffId) : IRequest<StaffMemberDetailDto>;

public class StaffMemberDetailDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? LastModified { get; set; }
    public int TenantId { get; set; }
}

public class GetStaffMemberQueryHandler : IRequestHandler<GetStaffMemberQuery, StaffMemberDetailDto>
{
    private readonly IAppDbContext _context;

    public GetStaffMemberQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StaffMemberDetailDto>> HandleAsync(GetStaffMemberQuery request, CancellationToken cancellationToken)
    {
        // Get the user (exclude soft deleted)
        var user = await _context.AppUser
            .SingleAsync(u => u.Id == request.StaffId, cancellationToken);

        // Get roles for the user (exclude soft deleted)
        var roles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId)
            .Select(r => r.Role)
            .ToListAsync(cancellationToken);

        var dto = new StaffMemberDetailDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Roles = roles,
            Created = user.Created,
            LastModified = user.LastModified,
            TenantId = user.TenantId ?? 0
        };

        return Result<StaffMemberDetailDto>.Success(dto);
    }
}
