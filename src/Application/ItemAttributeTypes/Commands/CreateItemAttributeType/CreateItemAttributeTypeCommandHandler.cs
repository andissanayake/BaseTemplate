using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributeTypes.Commands.CreateItemAttributeType;

public class CreateItemAttributeTypeCommandHandler : IRequestHandler<CreateItemAttributeTypeCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public CreateItemAttributeTypeCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();

        var itemAttributeType = new ItemAttributeType
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
            TenantId = userInfo.TenantId
        };

        _context.ItemAttributeType.Add(itemAttributeType);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(itemAttributeType.Id);
    }
} 