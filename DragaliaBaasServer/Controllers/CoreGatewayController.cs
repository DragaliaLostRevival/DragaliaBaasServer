using DragaliaBaasServer.Models.Core;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Route("core/v1")]
[Produces("application/json")]
public class CoreController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAccountService _accountService;
    private readonly IAuthorizationService _authorizationService;

    public CoreController(ILoggerProvider logger, IAccountService accountService,
        IAuthorizationService authorizationService)
    {
        _logger = logger.CreateLogger("Core");
        _accountService = accountService;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    [Consumes("application/json")]
    [Route("gateway/sdk/login")]
    public async Task<IActionResult> Login([FromBody] CoreRequest request)
    {
        var processResult = await _accountService.ProcessLoginRequestAsync(request);
        if (processResult == null)
            return BadRequest("Failed to process login request.");

        var (userAccount, createdDeviceAccount) = processResult.Value;

        if (!_authorizationService.TryLogin(userAccount, createdDeviceAccount, out var response))
        {
            _logger.LogError("Failed to login during login request. User account name: {userAccountName}", userAccount.Id);

            return BadRequest("Failed to login.");
        }

        return Ok(response);
    }

    [HttpPost]
    [Route("users/{userId:length(16)}/link")]
    public async Task<IActionResult> LinkAccount(string userId, [FromForm] string idp, [FromForm] string idToken)
    {
        string? authToken = HttpContext.Request.Headers.Authorization;

        if (
            string.IsNullOrEmpty(authToken)
            || !authToken.StartsWith("Bearer ")
            || !_authorizationService.TryParseToken(authToken[7..].Trim(), out var authUserId)
            || authUserId != userId
            || !_accountService.TryGetUserAccount(authUserId, out var userAccount))
        {
            return BadRequest("Invalid user account.");
        }

        if (!_authorizationService.TryParseToken(idToken, out var webAccountId)
            || !_accountService.TryGetWebAccount(webAccountId, out var webAccount))
        {
            _logger.LogWarning("Got link request with invalid IdentityProvider account. User account id: {uAccountId}", userAccount.Id);
            return BadRequest("Invalid identity provider account.");
        }

        if (userAccount.WebUserAccountId != null || _accountService.HasAssociatedUserAccount(webAccount))
            return BadRequest("Invalid account states for linking.");

        userAccount.WebUserAccountId = webAccount.Id;
        await _accountService.SaveChangesAsync();

        _logger.LogInformation("Successfully linked user account {uAccountId} to IdentityProvider account {idpAccountUsername}", userAccount.Id, webAccount.Id);

        return Ok(userAccount);
    }

    [HttpPost]
    [Consumes("application/json")]
    [Route("gateway/sdk/federation")]
    public async Task<IActionResult> MigrateDeviceAccountToUserAccount([FromBody] CoreRequest request)
    {
        if (request.DeviceAccount == null)
            return BadRequest("No device account provided.");

        if (request.IdpAccount == null)
            return BadRequest("No identity provider account provided.");

        if (request.PreviousUserId == null)
            return BadRequest("No previous user id provided.");

        if (!_authorizationService.TryParseToken(request.IdpAccount.IdToken, out var webAccountId)
            || !_accountService.TryGetWebAccount(webAccountId, out var webAccount))
        {
            _logger.LogWarning("Got federation request with invalid IdentityProvider account. Device account id: {dAccountId}", request.DeviceAccount.Id);
            return BadRequest("Invalid identity provider account.");
        }

        if (!_accountService.TryLoginDeviceAccount(request.DeviceAccount.Id, request.DeviceAccount.Password,
                out var oldDeviceAccount))
        {
            _logger.LogWarning("Got federation request with invalid device account. IdentityProvider account username: {idpAccountUsername}", webAccount.Username);
            return BadRequest("Invalid device account.");
        }

        if (!_accountService.TryAssociateDeviceAccountWithWeb(oldDeviceAccount, webAccount, out var newUser))
        {
            _logger.LogError("Failed to associated device account with IdentityProvider account: Device account id: {dAccountId}, IdentityProvider account username: {idpAccountUsername}", request.DeviceAccount.Id, webAccount.Username);
            return StatusCode(500);
        }

        await _accountService.SaveChangesAsync();

        if (!_authorizationService.TryLogin(newUser, null, out var response))
        {
            _logger.LogError(
                "Failed to login to new user account during federation. IdentityProvider account username: {idpAccountUsername}, Device account name: {dAccountName}",
                webAccount.Username, request.DeviceAccount.Id);

            return BadRequest("Failed to login after federation.");
        }

        _logger.LogInformation("Successfully federated device account {dAccountId} with IdentityProvider account {idpAccountUsername}", request.DeviceAccount.Id, webAccount.Id);

        return Ok(response);
    }
}