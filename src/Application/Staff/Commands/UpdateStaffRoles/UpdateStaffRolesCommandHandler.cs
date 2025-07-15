namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRoles;

public class UpdateStaffRolesCommandHandler : IRequestHandler<UpdateStaffRolesCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateStaffRolesCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRolesCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Verify the user exists and belongs to the tenant
        var user = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE id = @StaffId AND tenant_id = @TenantId",
            new { request.StaffId, request.TenantId });

        if (user == null)
        {
            return Result<bool>.Failure("Staff member not found or does not belong to this tenant.");
        }

        // Remove all existing roles for the user
        await uow.ExecuteAsync(
            "DELETE FROM user_role WHERE user_id = @StaffId",
            new { request.StaffId });

        // Add new roles
        if (request.NewRoles.Any())
        {
            var roleValues = request.NewRoles.Select(role => new { UserId = request.StaffId, Role = role });
            await uow.ExecuteAsync(
                "INSERT INTO user_role (user_id, role, created, last_modified) VALUES (@UserId, @Role, @Created, @LastModified)",
                roleValues.Select(r => new { r.UserId, r.Role, Created = DateTimeOffset.UtcNow, LastModified = DateTimeOffset.UtcNow }));
        }

        return Result<bool>.Success(true);
    }
} 