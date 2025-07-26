using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Infrastructure.Data;
internal class AppDbContext : DbContext, IAppDbContext
{
    private readonly IUser _user;
    public AppDbContext(DbContextOptions<AppDbContext> options, IUser user) : base(options) { _user = user; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<StaffRequest> StaffRequest { get; set; }
    public DbSet<StaffRequestRole> StaffRequestRole { get; set; }
    public DbSet<ItemAttributeType> ItemAttributeType { get; set; }
    public DbSet<ItemAttribute> ItemAttribute { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<Specification> Specification { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _user.Identifier;
                    entry.Entity.Created = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _user.Identifier;
                    entry.Entity.LastModified = DateTimeOffset.UtcNow;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

}
