namespace BaseTemplate.Application.Users.Queries;

[Authorize]
public record GetUserQuery : IRequest<GetUserResponse>
{
}
public record GetUserResponse
{
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public TenantDetails? Tenant { get; set; }
    public StaffRequestDetails? StaffRequest { get; set; }
}

public record TenantDetails
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public record StaffRequestDetails
{
    public int Id { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public StaffRequestStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public string TenantName { get; set; } = string.Empty;
}

public record UserWithTenantInfo
{
    public int Id { get; set; }
    public string SsoId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
}

public record StaffRequestBasicInfo
{
    public int Id { get; set; }
    public string RequestedBySsoId { get; set; } = string.Empty;
    public StaffRequestStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public string TenantName { get; set; } = string.Empty;
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;

    public GetUserQueryHandler(IUnitOfWorkFactory factory, IUser user)
    {
        _factory = factory;
        _user = user;
    }
    public async Task<Result<GetUserResponse>> HandleAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        var uow = _factory.Create();
        //Ensure the user exists in the database
        var userInfo = await uow.QueryFirstOrDefaultAsync<UserWithTenantInfo>(@"
            SELECT u.Id, u.sso_id, u.Name, u.Email, t.Id as Tenant_Id, t.Name as TenantName
            FROM app_user u
            LEFT JOIN Tenant t ON u.Tenant_Id = t.Id
            WHERE u.sso_id = @Identifier", new { _user.Identifier });

        if (userInfo == null)
        {
            var newAppUser = new AppUser() { SsoId = _user.Identifier!, Name = _user.Name, Email = _user.Email };
            await uow.InsertAsync(newAppUser);
            return Result<GetUserResponse>.Success(new GetUserResponse() { Roles = new List<string>() });
        }
        else
        {
            // Load minimal AppUser for update
            var existingUser = new AppUser { Id = userInfo.Id, SsoId = userInfo.SsoId };
            bool changed = false;
            if (userInfo.Name != _user.Name)
            {
                existingUser.Name = _user.Name;
                changed = true;
            }
            if (userInfo.Email != _user.Email)
            {
                existingUser.Email = _user.Email;
                changed = true;
            }

            if (changed)
            {
                await uow.UpdateAsync(existingUser);
            }
        }

        var roles = await uow.QueryAsync<string>("select role from user_role where user_sso_id = @Identifier", new { _user.Identifier });

        var response = new GetUserResponse { Roles = roles };

        // If user has a tenant, include tenant details
        if (!string.IsNullOrEmpty(userInfo?.TenantId))
        {
            response.Tenant = new TenantDetails
            {
                Id = userInfo.TenantId,
                Name = userInfo.TenantName ?? string.Empty
            };
        }
        else
        {
            // If user doesn't have a tenant, check for staff requests
            var staffRequestBasic = await uow.QueryFirstOrDefaultAsync<StaffRequestBasicInfo>(@"
                SELECT sr.id, sr.requested_by_sso_id, 
                       sr.status, sr.created, t.name as tenant_name
                FROM staff_request sr
                JOIN tenant t ON sr.tenant_id = t.id
                WHERE sr.requested_email = @Email AND sr.status = 0
                ORDER BY sr.created DESC
                LIMIT 1", new { Email = _user.Email });

            if (staffRequestBasic != null)
            {
                // Get requester's name and email
                var requesterInfo = await uow.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT name, email FROM app_user WHERE sso_id = @SsoId",
                    new { SsoId = staffRequestBasic.RequestedBySsoId });

                var staffRequest = new StaffRequestDetails
                {
                    Id = staffRequestBasic.Id,
                    RequesterName = requesterInfo?.name ?? string.Empty,
                    RequesterEmail = requesterInfo?.email ?? string.Empty,
                    Status = staffRequestBasic.Status,
                    Created = staffRequestBasic.Created,
                    TenantName = staffRequestBasic.TenantName
                };

                // Get roles for the staff request
                var staffRequestRoles = await uow.QueryAsync<string>(
                    "SELECT role FROM staff_request_role WHERE staff_request_id = @StaffRequestId",
                    new { StaffRequestId = staffRequest.Id });

                staffRequest.Roles = staffRequestRoles.ToList();
                response.StaffRequest = staffRequest;
            }
        }

        return Result<GetUserResponse>.Success(response);
    }
}
