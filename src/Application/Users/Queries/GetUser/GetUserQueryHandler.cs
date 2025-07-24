using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUser _user;
    private readonly IAppDbContext _context;

    public GetUserQueryHandler(IAppDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<GetUserResponse>> HandleAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        var response = new GetUserResponse();
        // Try to load user entity
        var user = await _context.AppUser
            .FirstOrDefaultAsync(u => u.SsoId == _user.Identifier && !u.IsDeleted, cancellationToken);

        // If not found, create user and use the new entity
        if (user == null)
        {
            user = new AppUser
            {
                SsoId = _user.Identifier,
                Name = _user.Name,
                Email = _user.Email
            };
            _context.AppUser.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (user.TenantId.HasValue)
        {
            var tenant = await _context.Tenant
                .Where(t => t.Id == user.TenantId.Value)
                .SingleAsync(cancellationToken);

            response.Tenant = new TenantDetails() { Id = tenant.Id, Name = tenant.Name };
        }

        var roles = await _context.UserRole
            .Where(r => r.UserId == user.Id && !r.IsDeleted)
            .Select(r => r.Role)
            .ToListAsync(cancellationToken);
        if (roles.Any(r => r == Roles.TenantOwner))
        {
            roles.AddRange(Roles.TenantBaseRoles);
        }

        response.Roles = roles;

        if (!user.TenantId.HasValue)
        {
            var staffRequest = await _context.StaffRequest
                .Where(sr => sr.RequestedEmail == _user.Email && sr.Status == StaffRequestStatus.Pending && !sr.IsDeleted)
                .OrderByDescending(sr => sr.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (staffRequest != null)
            {
                var requester = await _context.AppUser
                    .SingleAsync(u => u.SsoId == staffRequest.RequestedBySsoId, cancellationToken);

                var staffRequestRoles = await _context.StaffRequestRole
                    .Where(r => r.StaffRequestId == staffRequest.Id && !r.IsDeleted)
                    .Select(r => r.Role)
                    .ToListAsync(cancellationToken);

                var staffTenantName = await _context.Tenant
                    .Where(t => t.Id == staffRequest.TenantId)
                    .Select(t => t.Name)
                    .SingleAsync(cancellationToken);

                response.StaffRequest = new StaffRequestDetails
                {
                    Id = staffRequest.Id,
                    RequesterName = requester.Name!,
                    RequesterEmail = requester.Email!,
                    Status = staffRequest.Status,
                    Created = staffRequest.Created,
                    TenantName = staffTenantName,
                    Roles = staffRequestRoles
                };
            }
        }

        return Result<GetUserResponse>.Success(response);
    }
}
