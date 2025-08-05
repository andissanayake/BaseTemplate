using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Common;
using BaseTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Infrastructure.Data;

public class BaseDbContext(DbContextOptions<BaseDbContext> options) : DbContext(options), IBaseDbContext
{
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
public class AppDbContext(DbContextOptions<AppDbContext> options, IUserProfileService profileService) : DbContext(options), IAppDbContext
{
    private readonly IUserProfileService _profileService = profileService;

    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<UserRole> UserRole { get; set; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<StaffInvitation> StaffInvitation { get; set; }
    public DbSet<StaffInvitationRole> StaffInvitationRole { get; set; }
    public DbSet<CharacteristicType> CharacteristicType { get; set; }
    public DbSet<Characteristic> Characteristic { get; set; }
    public DbSet<Item> Item { get; set; }
    public DbSet<Specification> Specification { get; set; }
    public DbSet<ItemCharacteristicType> ItemCharacteristicType { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Combined query filters for soft delete and tenant isolation
        builder.Entity<AppUser>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<UserRole>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Tenant>().HasQueryFilter(e => !e.IsDeleted);

        builder.Entity<StaffInvitation>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<StaffInvitationRole>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<CharacteristicType>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<Characteristic>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<Item>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == _profileService.UserProfile.TenantId);
        builder.Entity<Specification>().HasQueryFilter(e => !e.IsDeleted && e.TenantId == _profileService.UserProfile.TenantId);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _profileService.UserProfile.Id;
                    entry.Entity.Created = DateTimeOffset.UtcNow;
                    if (entry.Entity is BaseTenantAuditableEntity tenantCreateEntity)
                    {
                        tenantCreateEntity.TenantId = _profileService.UserProfile.TenantId;
                    }
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _profileService.UserProfile.Id;
                    entry.Entity.LastModified = DateTimeOffset.UtcNow;
                    if (entry.Entity is BaseTenantAuditableEntity tenantEditEntity)
                    {
                        tenantEditEntity.TenantId = _profileService.UserProfile.TenantId;
                    }
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
