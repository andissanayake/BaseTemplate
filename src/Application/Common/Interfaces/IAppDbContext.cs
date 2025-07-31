using Microsoft.EntityFrameworkCore;
public interface IBaseDbContext
{
    DbSet<AppUser> AppUser { get; set; }
    DbSet<UserRole> UserRole { get; set; }
    DbSet<StaffRequest> StaffRequest { get; set; }
    DbSet<StaffRequestRole> StaffRequestRole { get; set; }
}
public interface IAppDbContext : IBaseDbContext
{
    DbSet<Tenant> Tenant { get; set; }
    DbSet<ItemAttributeType> ItemAttributeType { get; set; }
    DbSet<ItemAttribute> ItemAttribute { get; set; }
    DbSet<Item> Item { get; set; }
    DbSet<Specification> Specification { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
