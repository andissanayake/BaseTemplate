using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Specifications.Commands.UpdateSpecification;

public class UpdateSpecificationCommandHandler : IRequestHandler<UpdateSpecificationCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public UpdateSpecificationCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateSpecificationCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var specification = await _context.Specification
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.TenantId == userProfile.TenantId, cancellationToken);

        if (specification == null)
            return Result<bool>.NotFound($"Specification with ID {request.Id} not found.");

        specification.Name = request.Name;
        specification.Description = request.Description;
        specification.ParentSpecificationId = request.ParentSpecificationId;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
