using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Web;
using Microsoft.EntityFrameworkCore;

namespace DragaliaBaasServer.Database;

public class BaasDbContext : DbContext
{
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<DeviceAccount> Devices => Set<DeviceAccount>();
    public DbSet<WebUserAccount> WebUsers => Set<WebUserAccount>();

    public BaasDbContext(DbContextOptions<BaasDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>(builder =>
        {
            builder.Navigation(_ => _.WebUserAccount).AutoInclude();
            builder.Navigation(_ => _.AssociatedDeviceAccounts).AutoInclude();
        });
    }
}