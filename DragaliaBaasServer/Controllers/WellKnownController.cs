using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaBaasServer.Models.WellKnown;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Route(".well-known")]
public class WellKnownController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public WellKnownController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [Route("jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(_authorizationService.GetJwks());
    }

    [Route("openid-configuration")]
    public IActionResult GetOpenIdConfiguration()
    {
        return Ok(_authorizationService.GetOpenIdConnectConfiguration());
    }
}