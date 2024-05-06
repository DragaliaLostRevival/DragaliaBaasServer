using System.Text.Json;
using System.Text.Json.Serialization;
using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Route(".well-known")]
public class JwkController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public JwkController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [Route("jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(_authorizationService.GetJwks());
    }
}