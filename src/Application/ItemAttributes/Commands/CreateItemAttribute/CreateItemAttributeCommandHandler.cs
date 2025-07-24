using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Commands.CreateItemAttribute;

public class CreateItemAttributeCommandHandler : IRequestHandler<CreateItemAttributeCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public CreateItemAttributeCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();

        // Check if code already exists for this tenant
        var existingAttribute = await _context.ItemAttribute
            .FirstOrDefaultAsync(a => a.Code == request.Code && a.TenantId == userInfo.TenantId, cancellationToken);

        if (existingAttribute != null)
        {
            return Result<int>.Validation("Code must be unique within the tenant", []);
        }

        var itemAttribute = new ItemAttribute
        {
            Name = request.Name,
            Code = request.Code,
            Value = request.Value,
            ItemAttributeTypeId = request.ItemAttributeTypeId,
            TenantId = userInfo.TenantId,
            IsActive = true
        };

        _context.ItemAttribute.Add(itemAttribute);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(itemAttribute.Id);
    }
}
