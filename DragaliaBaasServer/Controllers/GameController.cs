using System.Net.Http.Headers;
using DragaliaBaasServer.Models.Game;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Route("gameplay/v1")]
public class GameController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAccountService _accountService;
    private readonly ISavefileService _savefileService;
    private readonly IAuthorizationService _authorizationService;

    public GameController(ILoggerProvider logger, IAccountService accountService, ISavefileService savefileService, IAuthorizationService authorizationService)
    {
        _logger = logger.CreateLogger("Game");
        _accountService = accountService;
        _savefileService = savefileService;
        _authorizationService = authorizationService;
    }

    [Route("savefile")]
    public async Task<IActionResult> GetSavefile([FromBody] SavefileRequest request)
    {
        if (request.IdToken == string.Empty
            || !_authorizationService.TryParseToken(request.IdToken, out var userId)
            || !_accountService.TryGetUserAccount(userId, out var userAccount)
           )
        {
            return BadRequest("Invalid user.");
        }

        if (userAccount.WebUserAccount is not {HasSavefile: true})
        {
            return BadRequest("User has no stored savefile.");
        }

        var stream = _savefileService.GetSavefile(userAccount.WebUserAccount);
        if (stream == null)
        {
            _logger.LogError("Failed to retrieve savefile for web user {webUserName}", userAccount.WebUserAccount.Username);
            return BadRequest("Failed to retrieve savefile.");
        }

        return File(stream, "application/json");
    }

    [Route("user")]
    public async Task<IActionResult> GetLinkedUser()
    {
        string? authToken = HttpContext.Request.Headers.Authorization;

        if (string.IsNullOrEmpty(authToken)
            || !authToken.StartsWith("Bearer ")
            || !_authorizationService.TryParseToken(authToken[7..].Trim(), out var webUserId))
        {
            return Unauthorized();
        }

        if (!_accountService.TryGetWebAccount(webUserId, out var webUser) || !_accountService.TryGetAssociatedUserAccount(webUser, out var userAccount))
        {
            return NotFound();
        }

        return Ok(new LinkedUserResponse(userAccount.Id));
    }

    [HttpGet("webUser")]
    public Results<UnauthorizedHttpResult, NotFound, Ok<WebUserResponse>> GetWebUser()
    {
        if (!AuthenticationHeaderValue.TryParse(HttpContext.Request.Headers.Authorization, out var authInfo)
            || authInfo.Scheme != "Bearer"
            || authInfo.Parameter == null
            || !_authorizationService.TryParseToken(authInfo.Parameter, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        if (!_accountService.TryGetUserAccount(userId, out var userAccount)
            || userAccount.WebUserAccount == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new WebUserResponse(userAccount.WebUserAccount.Id, userAccount.WebUserAccount.Username));
    }

    public record LinkedUserResponse(string UserId);

    public record WebUserResponse(string WebUserId, string Username);
}
