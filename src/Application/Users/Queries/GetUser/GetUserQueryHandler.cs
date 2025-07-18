namespace BaseTemplate.Application.Users.Queries.GetUser;

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
        using var uow = _factory.Create();

        // Load user profile from database
        var userProfile = await uow.QueryFirstOrDefaultAsync<UserWithTenantInfo>(@"
            SELECT u.Id, u.sso_id as SsoId, u.Name, u.Email, 
                   t.Id as TenantId, t.Name as TenantName
            FROM app_user u
            LEFT JOIN tenant t ON u.tenant_id = t.id
            WHERE u.sso_id = @SsoId", new { SsoId = _user.Identifier });

        if (userProfile == null)
        {
            // User does not exist, create new user
            var newAppUser = new AppUser() { SsoId = _user.Identifier!, Name = _user.Name, Email = _user.Email };
            await uow.InsertAsync(newAppUser);
            return Result<GetUserResponse>.Success(new GetUserResponse() { Roles = new List<string>() });
        }

        // Get user roles
        var roles = await uow.QueryAsync<string>(
            "SELECT role FROM user_role WHERE user_id = @UserId",
            new { UserId = userProfile.Id });

        var response = new GetUserResponse { Roles = roles.ToList() };

        // If user has a tenant, include tenant details from userProfile
        if (userProfile.TenantId != null && userProfile.TenantId.HasValue)
        {
            response.Tenant = new TenantDetails
            {
                Id = userProfile.TenantId.Value,
                Name = userProfile.TenantName ?? string.Empty
            };
        }
        else
        {
            // If user doesn't have a tenant, check for staff requests (keep this logic)
            var staffRequestBasic = await uow.QueryFirstOrDefaultAsync<StaffRequestBasicInfo>(@"
                SELECT sr.id, sr.requested_by_sso_id, 
                       sr.status, sr.created, t.name as tenant_name
                FROM staff_request sr
                JOIN tenant t ON sr.tenant_id = t.id
                WHERE sr.requested_email = @Email AND sr.status = 0
                ORDER BY sr.created DESC
                LIMIT 1", new { _user.Email });

            if (staffRequestBasic != null)
            {
                // Get requester's name and email
                var requesterInfo = await uow.QuerySingleAsync<dynamic>(
                    "SELECT name, email FROM app_user WHERE sso_id = @SsoId",
                    new { SsoId = staffRequestBasic.RequestedBySsoId });

                var staffRequest = new StaffRequestDetails
                {
                    Id = staffRequestBasic.Id,
                    RequesterName = requesterInfo.name,
                    RequesterEmail = requesterInfo.email,
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
