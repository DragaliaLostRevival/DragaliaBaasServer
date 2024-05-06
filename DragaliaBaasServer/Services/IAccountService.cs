using System.Diagnostics.CodeAnalysis;
using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Core;
using DragaliaBaasServer.Models.Web;

namespace DragaliaBaasServer.Services;

public interface IAccountService
{
    public bool TryCreateDeviceAccount([NotNullWhen(true)] out DeviceAccount? deviceAccount, [NotNullWhen(true)] out UserAccount? userAccount, DeviceAccount? existingAccount = null);
    public bool TryCreateUserAccount([NotNullWhen(true)] out UserAccount? userAccount);
    public bool TryCreateWebAccount(string id, string password, [NotNullWhen(true)] out WebUserAccount? webAccount);
    public bool TryGetUserAccount(string id, [NotNullWhen(true)] out UserAccount? userAccount);
    public bool TryGetWebAccount(string id, [NotNullWhen(true)] out WebUserAccount? webAccount);
    public bool TryLoginDeviceAccount(string id, string password, [NotNullWhen(true)] out DeviceAccount? deviceAccount);
    public bool TryLoginWebAccount(string id, string password, [NotNullWhen(true)] out WebUserAccount? webAccount);
    public bool TryAssociateDeviceAccountWithWeb(DeviceAccount deviceAccount, WebUserAccount webAccount, [NotNullWhen(true)] out UserAccount? userAccount);
    public bool DoesWebAccountWithUsernameExist(string username);
    public bool HasAssociatedUserAccount(WebUserAccount wAccount);
    public int GetAssociatedDeviceAccountsCount(WebUserAccount webAccount);
    public bool TryGetAssociatedUserAccount(WebUserAccount webAccount, [NotNullWhen(true)] out UserAccount? userAccount);
    public void UnlinkAccounts(WebUserAccount webAccount);
    public Task<(UserAccount uAccount, DeviceAccount? createdDeviceAccount)?> ProcessLoginRequestAsync(CoreRequest request);
    public Task SaveChangesAsync();
    public void SaveChanges();
}