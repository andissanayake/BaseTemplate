namespace BaseTemplate.Application.Staff.Commands.RemoveStaff;

public class RemoveStaffCommandHandler : IRequestHandler<RemoveStaffCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public RemoveStaffCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(RemoveStaffCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Verify the user exists and belongs to the tenant
        var user = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE sso_id = @StaffSsoId AND tenant_id = @TenantId",
            new { request.StaffSsoId, request.TenantId });

        if (user == null)
        {
            return Result<bool>.Failure("Staff member not found or does not belong to this tenant.");
        }

        // Remove all roles for the user
        await uow.ExecuteAsync(
            "DELETE FROM user_role WHERE user_sso_id = @StaffSsoId",
            new { request.StaffSsoId });

        // Remove the user from the tenant (set tenant_id to null)
        await uow.ExecuteAsync(
            "UPDATE app_user SET tenant_id = NULL, last_modified = @LastModified WHERE sso_id = @StaffSsoId",
            new { request.StaffSsoId, LastModified = DateTimeOffset.UtcNow });


        return Result<bool>.Success(true);
    }
} 