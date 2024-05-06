using System.Diagnostics.CodeAnalysis;
using DragaliaBaasServer.Database.Repositories;
using DragaliaBaasServer.Extensions;
using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Core;
using DragaliaBaasServer.Models.Vcm;
using DragaliaBaasServer.Models.Web;
using Microsoft.AspNetCore.Identity;

namespace DragaliaBaasServer.Services;

public class AccountService : IAccountService
{
    private readonly ILogger _logger;
    private readonly IAccountRepository _repository;
    private readonly Random _random;
    private readonly IPasswordHasher<IHasId> _passwordHasher;

    public AccountService(IAccountRepository repository, ILoggerFactory logger, IPasswordHasher<IHasId> passwordHasher)
    {
        _logger = logger.CreateLogger("Accounts");
        _repository = repository;
        _random = Random.Shared;
        _passwordHasher = passwordHasher;
    }

    public bool TryCreateDeviceAccount([NotNullWhen(true)] out DeviceAccount? deviceAccount, [NotNullWhen(true)] out UserAccount? userAccount, DeviceAccount? existingAccount = null)
    {
        deviceAccount = null;
        userAccount = null;

        DeviceAccount createdAccount;

        if (existingAccount != null)
        {
            if (_repository.DoesDeviceAccountExist(existingAccount.Id))
            {
                _logger.LogInformation("Tried to create device account with id {dAccountId} that already existed.", existingAccount.Id);
                return false;
            }

            createdAccount = existingAccount;
        }
        else
        {
            var id = _random.NextString();
            var password = _random.NextString(64);

            while (_repository.DoesDeviceAccountExist(id))
            {
                id = _random.NextString();
            }

            createdAccount = new DeviceAccount(id, password);
        }

        deviceAccount = new DeviceAccount(createdAccount.Id, createdAccount.Password);
        createdAccount.Password = _passwordHasher.HashPassword(createdAccount, createdAccount.Password);

        if (!TryCreateUserAccount(out userAccount))
            return false;

        userAccount.AssociatedDeviceAccounts.Add(createdAccount);
        _repository.AddDeviceAccount(createdAccount);

        _logger.LogInformation("Successfully created device account with id {dAccountId}", createdAccount.Id);

        return true;
    }

    public bool TryCreateUserAccount([NotNullWhen(true)] out UserAccount? userAccount)
    {
        var id = _random.NextString();

        while (TryGetUserAccount(id, out _))
        {
            id = _random.NextString();
        }

        userAccount = new UserAccount
        {
            Id = id,
            VcmInfo = Enum.GetValues<VcmMarket>().Select(market => new UserVcmInfo { Market = market }).ToList()
        };

        _repository.AddUserAccount(userAccount);

        _logger.LogInformation("Successfully created user account with id {uAccountId}", userAccount.Id);

        return true;
    }

    public bool TryCreateWebAccount(string username, string password, [NotNullWhen(true)] out WebUserAccount? webAccount)
    {
        webAccount = null;

        if (_repository.DoesWebUserAccountWithUsernameExist(username))
            return false;

        var id = _random.NextString();

        while (_repository.DoesWebUserAccountExist(id))
        {
            id = _random.NextString();
        }

        webAccount = new WebUserAccount
        {
            Username = username,
            Password = password,
            Id = id
        };

        webAccount.Password = _passwordHasher.HashPassword(webAccount, password);

        _repository.AddWebUserAccount(webAccount);

        _logger.LogInformation("Successfully created {wAccountUsername}", webAccount.Username);

        return true;
    }

    public bool TryGetUserAccount(string id, [NotNullWhen(true)] out UserAccount? userAccount)
    {
        userAccount = _repository.GetUserAccount(id);
        return userAccount != null;
    }

    public bool TryGetWebAccount(string id, [NotNullWhen(true)] out WebUserAccount? webAccount)
    {
        webAccount = _repository.GetWebUserAccount(id);
        return webAccount != null;
    }

    public bool TryLoginDeviceAccount(string id, string password, [NotNullWhen(true)] out DeviceAccount? deviceAccount)
    {
        deviceAccount = _repository.GetDeviceAccount(id);
        return deviceAccount != null && _passwordHasher.VerifyHashedPassword(deviceAccount, deviceAccount.Password, password) == PasswordVerificationResult.Success;
    }

    public bool TryLoginWebAccount(string username, string password, [NotNullWhen(true)] out WebUserAccount? webAccount)
    {
        webAccount = _repository.GetWebUserAccountByUsername(username);
        return webAccount != null && _passwordHasher.VerifyHashedPassword(webAccount, webAccount.Password, password) == PasswordVerificationResult.Success;
    }

    public bool TryAssociateDeviceAccountWithWeb(DeviceAccount deviceAccount, WebUserAccount webAccount, [NotNullWhen(true)] out UserAccount? userAccount)
    {
        var webUserAccount = _repository.GetAssociatedUserAccount(webAccount);
        var deviceUserAccount = _repository.GetAssociatedUserAccount(deviceAccount);

        if (webUserAccount == null)
        {
            // Need to link new baas/user account
            deviceUserAccount.WebUserAccountId = webAccount.Id;
            userAccount = deviceUserAccount;
            return true;
        }

        if (deviceUserAccount.WebUserAccountId != null && deviceUserAccount.WebUserAccountId == webUserAccount.WebUserAccountId)
        {
            userAccount = deviceUserAccount;
            return true;
        }

        deviceUserAccount.AssociatedDeviceAccounts.Remove(deviceAccount);
        webUserAccount.AssociatedDeviceAccounts.Add(deviceAccount);
        if (deviceUserAccount.WebUserAccountId == null && deviceUserAccount.AssociatedDeviceAccounts.Count == 0)
            _repository.RemoveUserAccount(deviceUserAccount);

        userAccount = webUserAccount;
        return true;
    }

    public bool DoesWebAccountWithUsernameExist(string username)
        => _repository.DoesWebUserAccountWithUsernameExist(username);

    public int GetAssociatedDeviceAccountsCount(WebUserAccount webAccount)
    {
        var associatedUser = _repository.GetAssociatedUserAccount(webAccount);
        return associatedUser == null
            ? 0
            : associatedUser.AssociatedDeviceAccounts.Count;
    }

    public bool TryGetAssociatedUserAccount(WebUserAccount webAccount, [NotNullWhen(true)] out UserAccount? userAccount)
    {
        userAccount = _repository.GetAssociatedUserAccount(webAccount);
        return userAccount != null;
    }

    public void UnlinkAccounts(WebUserAccount webAccount)
    {
        _logger.LogInformation("Unlinking {webAccountId} from all associated accounts.", webAccount.Id);

        var associatedUser = _repository.GetAssociatedUserAccount(webAccount);
        if (associatedUser == null)
        {
            _logger.LogInformation("No associated user account found.");
            return;
        }

        associatedUser.WebUserAccountId = null;
    }

    public async Task<(UserAccount uAccount, DeviceAccount? createdDeviceAccount)?> ProcessLoginRequestAsync(CoreRequest request)
    {
        UserAccount? userAccount;
        DeviceAccount? createdDeviceAccount = null;


        if (request.DeviceAccount == null || !_repository.DoesDeviceAccountExist(request.DeviceAccount.Id))
        {
            if (!TryCreateDeviceAccount(out createdDeviceAccount, out userAccount, request.DeviceAccount))
            {
                _logger.LogError("Failed to create new device account during login request.");
                return null;
            }
        }
        else
        {
            if (!TryLoginDeviceAccount(request.DeviceAccount.Id, request.DeviceAccount.Password,
                    out var deviceAccount))
            {
                _logger.LogInformation("Device tried to login with invalid DeviceAccount credentials.");
                return null;
            }

            userAccount = _repository.GetAssociatedUserAccount(deviceAccount);
        }

        await SaveChangesAsync();
        return (userAccount, createdDeviceAccount);
    }

    public bool HasAssociatedUserAccount(WebUserAccount wAccount)
        => _repository.GetUserAccounts().Any(uAccount => uAccount.WebUserAccountId == wAccount.Id);

    public async Task SaveChangesAsync()
        => await _repository.SaveChangesAsync();

    public void SaveChanges()
        => _repository.SaveChanges();
}