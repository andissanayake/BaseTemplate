using BaseTemplate.Domain.Constants;

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
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public GetStaffMemberQueryHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<StaffMemberDetailDto>> HandleAsync(GetStaffMemberQuery request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile?.TenantId == null)
        {
            return Result<StaffMemberDetailDto>.Forbidden("User is not associated with any tenant.");
        }

        var tenantId = userProfile.TenantId.Value;

        using var uow = _factory.Create();

        // Get the user
        var user = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE id = @StaffId AND tenant_id = @TenantId",
            new { StaffId = request.StaffId, TenantId = tenantId });

        if (user == null)
        {
            return Result<StaffMemberDetailDto>.Failure("Staff member not found or does not belong to this tenant.");
        }

        // Get roles for the user
        var roles = await uow.QueryAsync<UserRole>(
            "SELECT * FROM user_role WHERE user_id = @StaffId",
            new { StaffId = request.StaffId });

        var dto = new StaffMemberDetailDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Roles = roles.Select(r => r.Role).ToList(),
            Created = user.Created,
            LastModified = user.LastModified,
            TenantId = user.TenantId ?? 0
        };

        return Result<StaffMemberDetailDto>.Success(dto);
    }
}
