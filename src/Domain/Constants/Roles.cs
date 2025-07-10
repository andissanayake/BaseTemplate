namespace BaseTemplate.Domain.Constants;

public abstract class Roles
{
    public const string Administrator = nameof(Administrator);
    public const string TenantOwner = nameof(TenantOwner);
    
    // Section-specific roles for better control
    public const string ItemManager = nameof(ItemManager);
    public const string StaffRequestManager = nameof(StaffRequestManager);
    public const string StaffManager = nameof(StaffManager);
    public const string TenantManager = nameof(TenantManager);
}
