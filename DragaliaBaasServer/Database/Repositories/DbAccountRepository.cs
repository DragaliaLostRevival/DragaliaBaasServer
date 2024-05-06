using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Web;
using Microsoft.EntityFrameworkCore;

namespace DragaliaBaasServer.Database.Repositories;

public class DbAccountRepository : IAccountRepository
{
    private readonly BaasDbContext _dbContext;

    public DbAccountRepository(BaasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    #region DeviceAccount Methods

    public IQueryable<DeviceAccount> GetDeviceAccounts()
        => _dbContext.Devices;

    public DeviceAccount? GetDeviceAccount(string id)
        => _dbContext.Devices.Find(id);

    public void AddDeviceAccount(DeviceAccount account)
        => _dbContext.Devices.Add(account);

    public void RemoveDeviceAccount(DeviceAccount account)
        => _dbContext.Devices.Remove(account);

    public bool DoesDeviceAccountExist(string id)
        => _dbContext.Devices.Any(dAccount => dAccount.Id == id);

    public UserAccount GetAssociatedUserAccount(DeviceAccount account)
        => GetUserAccounts().First(uAccount => uAccount.AssociatedDeviceAccounts.Contains(account));

    #endregion
    #region UserAccount Methods

    public IQueryable<UserAccount> GetUserAccounts()
        => _dbContext.Users;

    public UserAccount? GetUserAccount(string id)
        => _dbContext.Users.Find(id);

    public void AddUserAccount(UserAccount account)
        => _dbContext.Users.Add(account);

    public void RemoveUserAccount(UserAccount account)
        => _dbContext.Users.Remove(account);

    #endregion
    #region WebUserAccount Methods

    public IQueryable<WebUserAccount> GetWebUserAccounts()
        => _dbContext.WebUsers;

    public WebUserAccount? GetWebUserAccount(string id)
        => _dbContext.WebUsers.Find(id);

    public WebUserAccount? GetWebUserAccountByUsername(string username)
        => _dbContext.WebUsers.FirstOrDefault(wAccount => wAccount.Username == username);

    public void AddWebUserAccount(WebUserAccount account)
        => _dbContext.WebUsers.Add(account);

    public void RemoveWebUserAccount(WebUserAccount account)
        => _dbContext.WebUsers.Remove(account);

    public bool DoesWebUserAccountWithUsernameExist(string username)
        => _dbContext.WebUsers.Any(wAccount => wAccount.Username == username);

    public bool DoesWebUserAccountExist(string id)
        => _dbContext.WebUsers.Any(wAccount => wAccount.Id == id);

    public UserAccount? GetAssociatedUserAccount(WebUserAccount account)
        => GetUserAccounts().FirstOrDefault(uAccount => uAccount.WebUserAccountId == account.Id);

    public bool DoesWebUserAccountWithPatreonIdExist(string patreonId)
        => _dbContext.WebUsers.Any(wAccount => wAccount.LinkedPatreonUserId == patreonId);

    #endregion

    public async Task SaveChangesAsync()
        => await _dbContext.SaveChangesAsync();

    public void SaveChanges()
        => _dbContext.SaveChanges();
}