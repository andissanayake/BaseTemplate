using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Items.Commands.DeleteItem;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public DeleteItemCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var entity = await _context.Item
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.TenantId == userProfile.TenantId, cancellationToken);

        if (entity is null)
        {
            return Result<bool>.NotFound($"Item with id {request.Id} not found.");
        }

        entity.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
