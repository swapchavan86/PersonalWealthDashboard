using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Infrastructure.Persistence;
namespace PersonalWealth.Api.Controllers;
[ApiController,Route("api/v1/investments"),Authorize]
public sealed class InvestmentsController(PersonalWealthDbContext db) : ControllerBase
{
    [HttpGet("accounts")] public async Task<IActionResult> Accounts(CancellationToken ct)=>Ok(await db.InvestmentAccounts.AsNoTracking().ToListAsync(ct));
    [HttpGet("securities")] public async Task<IActionResult> Securities(CancellationToken ct)=>Ok(await db.Securities.AsNoTracking().Where(x=>x.IsActive).ToListAsync(ct));
    [HttpGet("holdings")] public async Task<IActionResult> Holdings(CancellationToken ct)=>Ok(await db.InvestmentHoldings.AsNoTracking().Where(x=>x.Quantity>0).ToListAsync(ct));
}
