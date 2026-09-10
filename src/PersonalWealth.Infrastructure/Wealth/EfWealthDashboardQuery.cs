using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Wealth;
using PersonalWealth.Domain.Banking;
using PersonalWealth.Infrastructure.Persistence;
namespace PersonalWealth.Infrastructure.Wealth;
public sealed class EfWealthDashboardQuery(PersonalWealthDbContext db) : IWealthDashboardQuery
{
 public async Task<WealthDashboardDto> GetAsync(CancellationToken cancellationToken=default)
 {
  var cash=await db.BankTransactions.Where(x=>db.BankAccounts.Any(a=>a.Id==x.AccountId&&a.Status==BankAccountStatus.Active)).Select(x=>x.Direction==BankTransactionDirection.Credit?x.Amount:-x.Amount).SumAsync(cancellationToken);
  var investments=await db.InvestmentHoldings.SumAsync(x=>x.CostBasis,cancellationToken);
  var assets=await db.AssetValuations.GroupBy(x=>x.AssetId).Select(g=>g.OrderByDescending(x=>x.ValuationDate).Select(x=>x.Value).First()).SumAsync(cancellationToken);
  var liabilities=await db.Liabilities.Where(x=>x.IsActive).SumAsync(x=>x.OutstandingPrincipal,cancellationToken);
  return new WealthDashboardDto(cash,investments,assets,liabilities,cash+investments+assets-liabilities);
 }
}
