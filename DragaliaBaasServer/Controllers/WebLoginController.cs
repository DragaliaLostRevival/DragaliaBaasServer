using DragaliaBaasServer.Models;
using DragaliaBaasServer.Models.Web;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Produces("application/json")]
public class WebApiController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAccountService _accountService;
    private readonly IAuthorizationService _authorizationService;

    public WebApiController(ILoggerProvider logger, IAccountService accountService,
        IAuthorizationService authorizationService)
    {
        _logger = logger.CreateLogger("WebApi");
        _accountService = accountService;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    [Route("connect/1.0.0/api/session_token")]
    public IActionResult PostRedeemSessionTokenCode(
        [FromForm(Name = "client_id")] string clientId,
        [FromForm(Name = "session_token_code")] string sessionTokenCode,
        [FromForm(Name = "session_token_code_verifier")] string sessionTokenCodeVerifier
        )
    {
        if (!_authorizationService.TryRedeemSessionTokenCode(sessionTokenCode, sessionTokenCodeVerifier,
                out var webUserId))
        {
            _logger.LogInformation("Failed to redeem session token code.");
            return Unauthorized();
        }

        if (!_accountService.TryGetWebAccount(webUserId, out var webUser))
        {
            _logger.LogError("Redeemed session token code for user {webUserId} but the account does not exist!", webUserId);
            return StatusCode(500);
        }

        var sessionToken = _authorizationService.GenerateToken(webUser, AuthorizationTokenType.Session);

        return Ok(new SessionTokenInfo {Code = sessionTokenCode, SessionToken = sessionToken});
    }

    [Route("1.0.0/gateway/sdk/token")]
    [Consumes("application/json")]
    public IActionResult GetSdkToken([FromBody] SdkTokenRequest request)
    {
        if (!_authorizationService.TryParseToken(request.SessionToken, out var webUserId, false))
        {
            _logger.LogInformation("Tried to get SDK token but invalid session token was provided.");
            return Unauthorized();
        }

        if (!_accountService.TryGetWebAccount(webUserId, out var webUser))
        {
            _logger.LogError("Tried to get SDK token for user {webUserId} but the account does not exist!", webUserId);
            return StatusCode(500);
        }

        var sessionToken = _authorizationService.GenerateToken(webUser, AuthorizationTokenType.Session);
        var idToken = _authorizationService.GenerateToken(webUser, AuthorizationTokenType.Id);
        var accessToken = _authorizationService.GenerateToken(webUser, AuthorizationTokenType.Access);

        return Ok(new SdkTokenResponse(
            accessToken,
            idToken,
            sessionToken,
            new SdkUser(webUser),
            _authorizationService.GetTokenLifetime(AuthorizationTokenType.Access)
        ));
    }

}