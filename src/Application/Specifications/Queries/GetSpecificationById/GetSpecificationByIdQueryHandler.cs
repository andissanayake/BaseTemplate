using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Specifications.Queries.GetSpecificationById;

public class GetSpecificationByIdQueryHandler : IRequestHandler<GetSpecificationByIdQuery, GetSpecificationByIdResponse>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetSpecificationByIdQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<GetSpecificationByIdResponse>> HandleAsync(GetSpecificationByIdQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var specification = await _context.Specification
            .Include(s => s.ParentSpecification)
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.TenantId == userProfile.TenantId, cancellationToken);

        if (specification == null)
            return Result<GetSpecificationByIdResponse>.NotFound($"Specification with ID {request.Id} not found.");

        var response = new GetSpecificationByIdResponse
        {
            Id = specification.Id,
            Name = specification.Name,
            Description = specification.Description,
            ParentSpecificationId = specification.ParentSpecificationId,
            ParentSpecificationName = specification.ParentSpecification?.Name
        };

        return Result<GetSpecificationByIdResponse>.Success(response);
    }
}
