using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Items.Queries.GetItemById;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, ItemDto>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetItemByIdQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<ItemDto>> HandleAsync(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        var entity = await _context.Item
            .SingleAsync(i => i.Id == request.Id && i.TenantId == userInfo.TenantId && !i.IsDeleted, cancellationToken);

        var itemDto = new ItemDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            IsActive = entity.IsActive,
            Category = entity.Category
        };

        return Result<ItemDto>.Success(itemDto);
    }
}
