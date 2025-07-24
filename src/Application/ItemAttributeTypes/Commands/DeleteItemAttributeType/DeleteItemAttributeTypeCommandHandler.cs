using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributeTypes.Commands.DeleteItemAttributeType;

public class DeleteItemAttributeTypeCommandHandler : IRequestHandler<DeleteItemAttributeTypeCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public DeleteItemAttributeTypeCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();

        var itemAttributeType = await _context.ItemAttributeType
            .FirstOrDefaultAsync(t => t.Id == request.Id && t.TenantId == userInfo.TenantId, cancellationToken);

        if (itemAttributeType == null)
        {
            return Result<bool>.NotFound($"ItemAttributeType with id {request.Id} not found.");
        }

        itemAttributeType.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
