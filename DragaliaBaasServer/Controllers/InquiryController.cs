using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = DragaliaBaasServer.Services.IAuthorizationService;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Route("inquiry/v1")]
[Produces("application/json")]
public class InquiryController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAccountService _accountService;
    private readonly IAuthorizationService _authorizationService;

    public InquiryController(ILoggerProvider logger, IAccountService accountService, IAuthorizationService authorizationService)
    {
        _logger = logger.CreateLogger("Inquiry");
        _accountService = accountService;
        _authorizationService = authorizationService;
    }

    [HttpGet]
    [Route("users/{userId:length(16)}")]
    public IActionResult GetUserInquiryInfo(string userId)
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

        return Ok(userAccount.GetInquiryInfo());
    }
}