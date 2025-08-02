using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Commands.UpdateItemAttributeType;

public class UpdateItemAttributeTypeCommandHandler(IAppDbContext context) : IRequestHandler<UpdateItemAttributeTypeCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateItemAttributeTypeCommand request, CancellationToken cancellationToken)
    {

        var itemAttributeType = await _context.ItemAttributeType
            .SingleAsync(t => t.Id == request.Id, cancellationToken);

        itemAttributeType.Name = request.Name;
        itemAttributeType.Description = request.Description;
        itemAttributeType.IsActive = true;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
