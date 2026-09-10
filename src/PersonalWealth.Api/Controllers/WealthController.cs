using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWealth.Application.Wealth;
namespace PersonalWealth.Api.Controllers;
[ApiController]
[Route("api/v1/wealth")]
[Authorize]
public sealed class WealthController(IWealthDashboardQuery query) : ControllerBase
{
    [HttpGet("dashboard")]
    public Task<WealthDashboardDto> Dashboard(CancellationToken cancellationToken) => query.GetAsync(cancellationToken);
}
