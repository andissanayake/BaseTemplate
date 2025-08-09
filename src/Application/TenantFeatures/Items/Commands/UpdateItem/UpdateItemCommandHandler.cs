using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Items.Commands.UpdateItem;

public class UpdateItemCommandHandler(IAppDbContext context) : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Item
            .SingleAsync(i => i.Id == request.Id, cancellationToken);

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Tags = request.Tags;
        entity.IsActive = request.IsActive;
        entity.SpecificationId = request.SpecificationId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
