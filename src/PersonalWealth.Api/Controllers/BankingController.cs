using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Infrastructure.Persistence;
namespace PersonalWealth.Api.Controllers;
[ApiController,Route("api/v1/banking"),Authorize]
public sealed class BankingController(PersonalWealthDbContext db) : ControllerBase
{
    [HttpGet("accounts")]
    public async Task<IActionResult> Accounts(CancellationToken ct) => Ok(await db.BankAccounts.AsNoTracking().Select(x=>new{x.Id,x.Institution,x.AccountNumber,x.AccountType,x.Currency,x.Status}).ToListAsync(ct));
    [HttpGet("transactions")]
    public async Task<IActionResult> Transactions([FromQuery]int take=100,CancellationToken ct=default) => Ok(await db.BankTransactions.AsNoTracking().OrderByDescending(x=>x.TransactionDate).ThenByDescending(x=>x.Id).Take(Math.Clamp(take,1,500)).Select(x=>new{x.Id,x.AccountId,x.TransactionDate,x.Amount,x.Direction,x.Description,x.Category,x.TransferId}).ToListAsync(ct));
}
