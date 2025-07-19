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
        var userProfile = await GetUserProfileAsync(uow);

        if (userProfile == null)
        {
            // User does not exist, create new user
            var id = await CreateNewUserAsync(uow);
            userProfile = await GetUserProfileAsync(uow);
        }

        // Get user roles
        var roles = await GetUserRolesAsync(uow, userProfile!.Id);
        var response = new GetUserResponse { Roles = roles.ToList() };

        // If user has a tenant, include tenant details from userProfile
        if (userProfile.TenantId.HasValue)
        {
            response.Tenant = new TenantDetails() { Id = userProfile.TenantId.Value, Name = userProfile.TenantName ?? "" };
        }
        else
        {
            // If user doesn't have a tenant, check for staff requests
            var staffRequest = await GetStaffRequestAsync(uow, _user.Email);
            if (staffRequest != null)
            {
                response.StaffRequest = staffRequest;
            }
        }

        return Result<GetUserResponse>.Success(response);
    }

    private async Task<UserWithTenantInfo?> GetUserProfileAsync(IUnitOfWork uow)
    {
        return await uow.QueryFirstOrDefaultAsync<UserWithTenantInfo>(@"
            SELECT u.Id, u.sso_id as SsoId, u.Name, u.Email, 
                   t.Id as TenantId, t.Name as TenantName
            FROM app_user u
            LEFT JOIN tenant t ON u.tenant_id = t.id
            WHERE u.sso_id = @SsoId", new { SsoId = _user.Identifier });
    }

    private async Task<int> CreateNewUserAsync(IUnitOfWork uow)
    {
        var newAppUser = new AppUser()
        {
            SsoId = _user.Identifier,
            Name = _user.Name,
            Email = _user.Email
        };

        return await uow.InsertAsync(newAppUser);
    }

    private async Task<IEnumerable<string>> GetUserRolesAsync(IUnitOfWork uow, int userId)
    {
        var roles = await uow.QueryAsync<string>(
            "SELECT role FROM user_role WHERE user_id = @UserId",
            new { UserId = userId });
        if (roles.Any(r => r == Roles.TenantOwner))
        {
            var rolesList = roles.ToList();
            rolesList.AddRange(Roles.TenantBaseRoles);
            roles = rolesList;
        }
        return roles;
    }

    private async Task<StaffRequestDetails?> GetStaffRequestAsync(IUnitOfWork uow, string email)
    {
        var staffRequestBasic = await GetStaffRequestBasicAsync(uow, email);

        if (staffRequestBasic == null)
        {
            return null;
        }

        var requesterInfo = await GetRequesterInfoAsync(uow, staffRequestBasic.RequestedBySsoId);
        var staffRequestRoles = await GetStaffRequestRolesAsync(uow, staffRequestBasic.Id);

        return new StaffRequestDetails
        {
            Id = staffRequestBasic.Id,
            RequesterName = requesterInfo.name,
            RequesterEmail = requesterInfo.email,
            Status = staffRequestBasic.Status,
            Created = staffRequestBasic.Created,
            TenantName = staffRequestBasic.TenantName,
            Roles = staffRequestRoles.ToList()
        };
    }

    private async Task<StaffRequestBasicInfo?> GetStaffRequestBasicAsync(IUnitOfWork uow, string email)
    {
        return await uow.QueryFirstOrDefaultAsync<StaffRequestBasicInfo>(@"
            SELECT sr.id, sr.requested_by_sso_id, 
                   sr.status, sr.created, t.name as tenant_name
            FROM staff_request sr
            JOIN tenant t ON sr.tenant_id = t.id
            WHERE sr.requested_email = @Email AND sr.status = 0
            ORDER BY sr.created DESC
            LIMIT 1", new { Email = email });
    }

    private async Task<dynamic> GetRequesterInfoAsync(IUnitOfWork uow, string ssoId)
    {
        return await uow.QuerySingleAsync<dynamic>(
            "SELECT name, email FROM app_user WHERE sso_id = @SsoId",
            new { SsoId = ssoId });
    }

    private async Task<IEnumerable<string>> GetStaffRequestRolesAsync(IUnitOfWork uow, int staffRequestId)
    {
        return await uow.QueryAsync<string>(
            "SELECT role FROM staff_request_role WHERE staff_request_id = @StaffRequestId",
            new { StaffRequestId = staffRequestId });
    }
}
