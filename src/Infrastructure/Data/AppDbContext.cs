using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Infrastructure.Data;

public class BaseDbContext : DbContext, IBaseDbContext
{
    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) { }
    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<StaffRequest> StaffRequest { get; set; }
    public DbSet<StaffRequestRole> StaffRequestRole { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure soft delete filters for base entities
        builder.Entity<AppUser>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<UserRole>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Tenant>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffRequest>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffRequestRole>().HasQueryFilter(e => !e.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModified = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
public class AppDbContext : DbContext, IAppDbContext
{
    private readonly IUserProfileService _profileService;

    public AppDbContext(DbContextOptions<AppDbContext> options, IUserProfileService profileService) : base(options)
    {
        _profileService = profileService;
    }

    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<StaffRequest> StaffRequest { get; set; }
    public DbSet<StaffRequestRole> StaffRequestRole { get; set; }
    public DbSet<ItemAttributeType> ItemAttributeType { get; set; }
    public DbSet<ItemAttribute> ItemAttribute { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<Specification> Specification { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<UserRole>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Tenant>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffRequest>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffRequestRole>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ItemAttributeType>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ItemAttribute>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Item>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Specification>().HasQueryFilter(e => !e.IsDeleted);

        builder.Entity<AppUser>().HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
        builder.Entity<StaffRequest>().HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
        builder.Entity<StaffRequestRole>().HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
        builder.Entity<ItemAttributeType>().HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
        builder.Entity<ItemAttribute>().HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
        builder.Entity<Item>().HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
        builder.Entity<Specification>().HasQueryFilter(e => e.TenantId == GetCurrentTenantId());
    }

    private int GetCurrentTenantId()
    {
        var userProfile = _profileService.GetUserProfileAsync().GetAwaiter().GetResult();
        if (userProfile.TenantId < 1)
        {
            throw new InvalidOperationException("There should be a tenant id to access application db context.");
        }
        return userProfile.TenantId;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var userProfile = await _profileService.GetUserProfileAsync();

        foreach (var entry in ChangeTracker.Entries<BaseTenantAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.TenantId = userProfile.TenantId;
                    entry.Entity.CreatedBy = userProfile.Identifier;
                    entry.Entity.Created = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.TenantId = userProfile.TenantId;
                    entry.Entity.LastModifiedBy = userProfile.Identifier;
                    entry.Entity.LastModified = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
