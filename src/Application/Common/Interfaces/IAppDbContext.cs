using Microsoft.EntityFrameworkCore;
public interface IBaseDbContext
{
    DbSet<AppUser> AppUser { get; set; }
    DbSet<UserRole> UserRole { get; set; }
    DbSet<Tenant> Tenant { get; set; }
    DbSet<StaffInvitation> StaffInvitation { get; set; }
    DbSet<StaffInvitationRole> StaffInvitationRole { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
public interface IAppDbContext : IBaseDbContext
{
    DbSet<CharacteristicType> CharacteristicType { get; set; }
    DbSet<Characteristic> Characteristic { get; set; }
    DbSet<Item> Item { get; set; }
    DbSet<Specification> Specification { get; set; }
    DbSet<ItemCharacteristicType> ItemCharacteristicType { get; set; }
    DbSet<ItemVariant> ItemVariant { get; set; }
    DbSet<ItemVariantCharacteristic> ItemVariantCharacteristic { get; set; }
}
