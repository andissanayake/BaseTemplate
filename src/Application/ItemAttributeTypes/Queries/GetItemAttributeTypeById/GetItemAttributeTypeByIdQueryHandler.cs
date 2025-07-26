using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypeById;

public class GetItemAttributeTypeByIdQueryHandler : IRequestHandler<GetItemAttributeTypeByIdQuery, ItemAttributeTypeDto>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetItemAttributeTypeByIdQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<ItemAttributeTypeDto>> HandleAsync(GetItemAttributeTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var userInfo = await _userProfileService.GetUserProfileAsync();
        var itemAttributeType = await _context.ItemAttributeType
            .SingleAsync(iat => iat.Id == request.Id && iat.TenantId == userInfo.TenantId && !iat.IsDeleted, cancellationToken);

        var dto = new ItemAttributeTypeDto
        {
            Id = itemAttributeType.Id,
            Name = itemAttributeType.Name,
            Description = itemAttributeType.Description,
            IsActive = itemAttributeType.IsActive
        };

        return Result<ItemAttributeTypeDto>.Success(dto);
    }
}
