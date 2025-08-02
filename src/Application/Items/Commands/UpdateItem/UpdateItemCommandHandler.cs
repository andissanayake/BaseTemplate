using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Items.Commands.UpdateItem;

public class UpdateItemCommandHandler(IAppDbContext context) : IRequestHandler<UpdateItemCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Item
            .SingleAsync(i => i.Id == request.Id, cancellationToken);

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.Category = request.Category;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
