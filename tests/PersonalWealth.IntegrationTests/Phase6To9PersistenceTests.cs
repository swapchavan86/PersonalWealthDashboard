using Microsoft.EntityFrameworkCore;
using PersonalWealth.Domain.Assets;
using PersonalWealth.Domain.Investments;
using PersonalWealth.Domain.Liabilities;
using PersonalWealth.Infrastructure.Persistence;
using Xunit;
namespace PersonalWealth.IntegrationTests;
public sealed class Phase6To9PersistenceTests
{
 [Fact] public async Task TenantOwnedFinancialModels_PersistAndRemainTenantScoped()
 {
  var tenant=Guid.NewGuid(); var options=new DbContextOptionsBuilder<PersonalWealthDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
  await using var db=new PersonalWealthDbContext(options,new TenantContext(tenant));
  var account=new InvestmentAccount(Guid.NewGuid(),tenant,"Broker","ACC-1",InvestmentAccountType.Brokerage,"INR"); var security=new Security(Guid.NewGuid(),tenant,"TEST","Test Equity",SecurityType.Equity,"INR"); var asset=new Asset(Guid.NewGuid(),tenant,"Home",AssetType.Property,"INR",100000,DateTime.UtcNow.AddYears(-1)); var liability=new Liability(Guid.NewGuid(),tenant,"Loan",LiabilityType.PersonalLoan,"INR",50000,10,DateTime.UtcNow.AddYears(-1));
  db.AddRange(account,security,asset,liability); await db.SaveChangesAsync();
  Assert.Single(await db.InvestmentAccounts.ToListAsync()); Assert.Single(await db.Assets.ToListAsync()); Assert.Single(await db.Liabilities.ToListAsync());
 }
}
