using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Commands.UpdateItemAttribute;

public class UpdateItemAttributeCommandHandler : IRequestHandler<UpdateItemAttributeCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public UpdateItemAttributeCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemAttributeCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();

        var itemAttribute = await _context.ItemAttribute
            .SingleAsync(a => a.Id == request.Id && a.TenantId == userInfo.TenantId, cancellationToken);

        // Check if code already exists for this tenant (excluding current item)
        var existingAttribute = await _context.ItemAttribute
            .FirstOrDefaultAsync(a => a.Code == request.Code && a.TenantId == userInfo.TenantId && a.Id != request.Id, cancellationToken);

        if (existingAttribute != null)
        {
            return Result<bool>.Validation("Code must be unique within the tenant",
                                                new Dictionary<string, string[]>
                                                {
                                                    ["Code"] = new[] { $"Code must be unique within the tenant." }
                                                });
        }

        itemAttribute.Name = request.Name;
        itemAttribute.Code = request.Code;
        itemAttribute.Value = request.Value;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
