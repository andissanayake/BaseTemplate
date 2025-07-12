using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Staff.Queries.GetStaffMember;

[Authorize(Roles = Roles.StaffManager)]
public record GetStaffMemberQuery(int TenantId, string StaffSsoId) : BaseTenantRequest<StaffMemberDetailDto>(TenantId);

public class StaffMemberDetailDto
{
    public string SsoId { get; set; } = string.Empty;
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

    public GetStaffMemberQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<StaffMemberDetailDto>> HandleAsync(GetStaffMemberQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Get the user
        var user = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE sso_id = @StaffSsoId AND tenant_id = @TenantId",
            new { request.StaffSsoId, request.TenantId });

        if (user == null)
        {
            return Result<StaffMemberDetailDto>.Failure("Staff member not found or does not belong to this tenant.");
        }

        // Get roles for the user
        var roles = await uow.QueryAsync<UserRole>(
            "SELECT * FROM user_role WHERE user_sso_id = @StaffSsoId",
            new { request.StaffSsoId });

        var dto = new StaffMemberDetailDto
        {
            SsoId = user.SsoId,
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
