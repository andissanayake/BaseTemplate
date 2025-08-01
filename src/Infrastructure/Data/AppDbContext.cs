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
    public DbSet<StaffInvitation> StaffInvitation { get; set; }
    public DbSet<StaffInvitationRole> StaffInvitationRole { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure soft delete filters for base entities
        builder.Entity<AppUser>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<UserRole>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Tenant>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffInvitation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffInvitationRole>().HasQueryFilter(e => !e.IsDeleted);
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
    public DbSet<StaffInvitation> StaffInvitation { get; set; }
    public DbSet<StaffInvitationRole> StaffInvitationRole { get; set; }
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
        builder.Entity<StaffInvitation>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<StaffInvitationRole>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ItemAttributeType>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<ItemAttribute>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Item>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Specification>().HasQueryFilter(e => !e.IsDeleted);

        builder.Entity<AppUser>().HasQueryFilter(e => e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<StaffInvitation>().HasQueryFilter(e => e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<StaffInvitationRole>().HasQueryFilter(e => e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<ItemAttributeType>().HasQueryFilter(e => e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<ItemAttribute>().HasQueryFilter(e => e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<Item>().HasQueryFilter(e => e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<Specification>().HasQueryFilter(e => e.TenantId == _profileService.UserProfile.TenantId);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<BaseTenantAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.TenantId = _profileService.UserProfile.TenantId;
                    entry.Entity.CreatedBy = _profileService.UserProfile.Id;
                    entry.Entity.Created = DateTimeOffset.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.TenantId = _profileService.UserProfile.TenantId;
                    entry.Entity.LastModifiedBy = _profileService.UserProfile.Id;
                    entry.Entity.LastModified = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
