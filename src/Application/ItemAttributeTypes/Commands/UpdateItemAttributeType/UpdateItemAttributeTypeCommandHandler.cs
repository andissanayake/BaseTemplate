using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributeTypes.Commands.UpdateItemAttributeType;

public class UpdateItemAttributeTypeCommandHandler : IRequestHandler<UpdateItemAttributeTypeCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public UpdateItemAttributeTypeCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();

        var itemAttributeType = await _context.ItemAttributeType
            .SingleAsync(t => t.Id == request.Id && t.TenantId == userInfo.TenantId, cancellationToken);

        itemAttributeType.Name = request.Name;
        itemAttributeType.Description = request.Description;
        itemAttributeType.IsActive = true;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
