using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Items.Commands.CreateItem;

public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;
    public CreateItemCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateItemCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();

        var entity = new Item
        {
            TenantId = userInfo.TenantId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            IsActive = true
        };

        _context.Item.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(entity.Id);
    }
}
