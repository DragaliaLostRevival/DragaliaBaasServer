using DragaliaBaasServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Produces("application/json")]
[Route("patreon")]
public class PatreonController : ControllerBase
{
    private readonly PatreonService _patreonService;

    public PatreonController(PatreonService patreonService)
    {
        _patreonService = patreonService;
    }

    [HttpGet("auth")]
    public async Task<IActionResult> Auth(string code, string state)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            return Unauthorized();

        var result = await _patreonService.LinkPatreonAccount(code, state);

        return Redirect($"{Constants.ServerUrl}/linkresult?patreonResult={result}");
    }
}