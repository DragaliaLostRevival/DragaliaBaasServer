using DragaliaBaasServer.Models.Backend;
using DragaliaBaasServer.Models.Vcm;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Route("vcm/v1")]
[Produces("application/json")]
[Consumes("application/json")]
public class VcmController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IAuthorizationService _authorizationService;

    public VcmController(IAccountService accountService, IAuthorizationService authorizationService)
    {
        _accountService = accountService;
        _authorizationService = authorizationService;
    }

    /*[HttpGet]
    [Route("markets/{marketName:alpha}/bundles")]
    public IActionResult GetVcmBundleInfoForMarket(string marketName)
    {
        var fuck = "this endpoint";
        return Ok(new
        {
            fuck
        });

        if (Enum.TryParse(marketName, true, out VcmMarket market))
            return Ok(VcmBundle.GetEmptyBundlesForMarket(market));

        return BadRequest("Invalid market.");
    }

    [HttpGet]
    [Route("users/{userId:length(16)}/markets/{marketName:alpha}/wallets")]
    public IActionResult GetVcmWalletInfoForMarket(string userId, string marketName)
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

        if (Enum.TryParse(marketName, true, out VcmMarket market))
            return Ok(userAccount.GetVcmWalletsForMarket(market));

        return BadRequest("Invalid market.");
    }*/
}