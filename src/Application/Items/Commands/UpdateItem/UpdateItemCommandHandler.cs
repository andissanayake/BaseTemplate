using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Items.Commands.UpdateItem;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public UpdateItemCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        var entity = await _context.Item
            .FirstOrDefaultAsync(i => i.Id == request.Id && i.TenantId == userInfo.TenantId, cancellationToken);

        if (entity == null)
        {
            return Result<bool>.NotFound($"Item with id {request.Id} not found.");
        }

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Category = request.Category;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
