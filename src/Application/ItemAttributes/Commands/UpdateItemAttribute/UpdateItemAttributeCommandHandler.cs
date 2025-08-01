using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.ItemAttributes.Commands.UpdateItemAttribute;

public class UpdateItemAttributeCommandHandler : IRequestHandler<UpdateItemAttributeCommand, bool>
{
    private readonly IAppDbContext _context;
    public UpdateItemAttributeCommandHandler(IAppDbContext context)
    {
        _context = context; ;
    }

    public async Task<Result<bool>> HandleAsync(UpdateItemAttributeCommand request, CancellationToken cancellationToken)
    {

        var itemAttribute = await _context.ItemAttribute
            .SingleAsync(a => a.Id == request.Id, cancellationToken);

        // Check if code already exists for this tenant (excluding current item)
        var existingAttribute = await _context.ItemAttribute
            .FirstOrDefaultAsync(a => a.Code == request.Code && a.Id != request.Id, cancellationToken);

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
