using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Web;

namespace DragaliaBaasServer.Database.Repositories;

public interface IAccountRepository
{
    public IQueryable<DeviceAccount> GetDeviceAccounts();
    public DeviceAccount? GetDeviceAccount(string id);
    public void AddDeviceAccount(DeviceAccount account);
    public void RemoveDeviceAccount(DeviceAccount account);
    public bool DoesDeviceAccountExist(string id);
    public UserAccount GetAssociatedUserAccount(DeviceAccount account);

    public IQueryable<UserAccount> GetUserAccounts();
    public UserAccount? GetUserAccount(string id);
    public void AddUserAccount(UserAccount account);
    public void RemoveUserAccount(UserAccount account);

    public IQueryable<WebUserAccount> GetWebUserAccounts();
    public WebUserAccount? GetWebUserAccount(string id);
    public WebUserAccount? GetWebUserAccountByUsername(string username);
    public void AddWebUserAccount(WebUserAccount account);
    public void RemoveWebUserAccount(WebUserAccount account);
    public bool DoesWebUserAccountWithUsernameExist(string username);
    public bool DoesWebUserAccountExist(string id);
    public UserAccount? GetAssociatedUserAccount(WebUserAccount account);
    public bool DoesWebUserAccountWithPatreonIdExist(string patreonId);

    public Task SaveChangesAsync();
    public void SaveChanges();
}