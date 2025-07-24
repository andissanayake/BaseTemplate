using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Commands.DeleteItemAttribute;

public class DeleteItemAttributeCommandHandler : IRequestHandler<DeleteItemAttributeCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public DeleteItemAttributeCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();

        var itemAttribute = await _context.ItemAttribute
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.TenantId == userInfo.TenantId, cancellationToken);

        if (itemAttribute == null)
        {
            return Result<bool>.NotFound($"ItemAttribute with id {request.Id} not found.");
        }

        itemAttribute.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
