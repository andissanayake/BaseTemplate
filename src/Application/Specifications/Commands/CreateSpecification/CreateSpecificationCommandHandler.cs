namespace BaseTemplate.Application.Specifications.Commands.CreateSpecification;

public class CreateSpecificationCommandHandler : IRequestHandler<CreateSpecificationCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public CreateSpecificationCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateSpecificationCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var specification = new Specification
        {
            Name = request.Name,
            Description = request.Description,
            ParentSpecificationId = request.ParentSpecificationId,
            TenantId = userProfile.TenantId
        };

        _context.Specification.Add(specification);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(specification.Id);
    }
}
