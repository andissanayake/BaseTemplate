using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.GlobalFeatures.Users.Commands.GetUser;

public class GetUserCommandHandler(IBaseDbContext context, IUser user) : IRequestHandler<GetUserCommand, GetUserResponse>
{
    private readonly IUser _user = user;
    private readonly IBaseDbContext _context = context;

    public async Task<Result<GetUserResponse>> HandleAsync(GetUserCommand request, CancellationToken cancellationToken)
    {
        var response = new GetUserResponse();
        var user = await _context.AppUser.AsNoTracking()
            .FirstOrDefaultAsync(u => u.SsoId == _user.Identifier, cancellationToken);

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
            var tenant = await _context.Tenant.AsNoTracking()
                .Where(t => t.Id == user.TenantId.Value)
                .SingleAsync(cancellationToken);

            response.Tenant = new TenantDetails() { Id = tenant.Id, Name = tenant.Name };
        }
        else
        {
            var staffRequest = await _context.StaffInvitation.AsNoTracking()
                .Include(sr => sr.RequestedByAppUser)
                .Where(sr => sr.RequestedEmail == _user.Email && sr.Status == StaffInvitationStatus.Pending)
                .OrderByDescending(sr => sr.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (staffRequest != null)
            {
                var staffRequestRoles = await _context.StaffInvitationRole.AsNoTracking()
                    .Where(r => r.StaffInvitationId == staffRequest.Id)
                    .Select(r => r.Role)
                    .ToListAsync(cancellationToken);

                var staffTenantName = await _context.Tenant.AsNoTracking()
                    .Where(t => t.Id == staffRequest.TenantId)
                    .Select(t => t.Name)
                    .SingleAsync(cancellationToken);

                response.StaffInvitation = new StaffInvitationDetails
                {
                    Id = staffRequest.Id,
                    RequesterName = staffRequest.RequestedByAppUser.Name!,
                    RequesterEmail = staffRequest.RequestedByAppUser.Email!,
                    Status = staffRequest.Status,
                    Created = staffRequest.Created,
                    TenantName = staffTenantName,
                    Roles = staffRequestRoles
                };
            }
        }
        var roles = await _context.UserRole.AsNoTracking()
            .Where(r => r.UserId == user.Id)
            .Select(r => r.Role)
            .ToListAsync(cancellationToken);

        if (roles.Any(r => r == Roles.TenantOwner))
        {
            roles.AddRange(Roles.TenantBaseRoles);
        }

        response.Roles = roles;

        return Result<GetUserResponse>.Success(response);
    }
}
