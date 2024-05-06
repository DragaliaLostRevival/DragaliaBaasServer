using DragaliaBaasServer.Models.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace DragaliaBaasServer.Controllers;

[ApiController]
[Route("bigdata/v1/analytics/events")]
public class AnalyticsController : ControllerBase
{
    [HttpPost]
    [Route("")]
    public IActionResult SubmitAnalyticsInfo()
    {
        // :ok: :cool:
        return Accepted();
    }

    [HttpGet]
    [Route("config")]
    [Produces("application/json")]
    public IActionResult GetAnalyticsConfig()
    {
        // all my homies hate analytics
        return Ok(new AnalyticsConfig());
    }
}