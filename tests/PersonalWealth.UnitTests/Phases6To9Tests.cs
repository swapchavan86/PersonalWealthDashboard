using PersonalWealth.Application.Assets;
using PersonalWealth.Application.Investments;
using PersonalWealth.Application.Liabilities;
using PersonalWealth.Application.Wealth;
using PersonalWealth.Domain.Assets;
using PersonalWealth.Domain.Investments;
using PersonalWealth.Domain.Liabilities;
using Xunit;
namespace PersonalWealth.UnitTests;
public sealed class Phases6To9Tests
{
    [Fact] public void CorporateAction_ExposesQuantityMultiplier() { var a=new CorporateAction(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid(),CorporateActionType.StockSplit,DateTime.UtcNow,5,1); Assert.Equal(5,a.QuantityMultiplier); }
    [Fact] public void Reconciliation_DetectsVariance() { var t=Guid.NewGuid();var ac=Guid.NewGuid();var s=Guid.NewGuid();var e=new InvestmentHolding(Guid.NewGuid(),t,ac,s);e.ApplyBuy(10,100,0);var a=new InvestmentHolding(Guid.NewGuid(),t,ac,s);a.ApplyBuy(9,100,0);var r=new InvestmentReconciliationService().Reconcile(new[]{e},new[]{a});Assert.False(r.IsBalanced); }
    [Fact] public void AssetValuation_UsesLatestDate() { var t=Guid.NewGuid();var asset=new Asset(Guid.NewGuid(),t,"Home",AssetType.Property,"INR",100,DateTime.UtcNow.AddYears(-1));var v1=new AssetValuation(Guid.NewGuid(),t,asset.Id,DateTime.UtcNow.AddMonths(-2),110,"manual");var v2=new AssetValuation(Guid.NewGuid(),t,asset.Id,DateTime.UtcNow.AddMonths(-1),125,"manual");Assert.Equal(125,new AssetValuationService().GetLatest(asset,new[]{v1,v2}).Value); }
    [Fact] public void LoanSchedule_ReducesBalance() { var l=new Liability(Guid.NewGuid(),Guid.NewGuid(),"Loan",LiabilityType.PersonalLoan,"INR",10000,12,DateTime.UtcNow);var lines=new LoanScheduleService().Build(l,1000,12);Assert.True(lines.Last().Balance<10000); }
    [Fact] public void WealthEngine_CalculatesNetWorthAndSavings() { var r=new WealthEngine().Calculate(new WealthInputs(100,500,200,300,1000,400,600,DateTime.UtcNow));Assert.Equal(500,r.NetWorth);Assert.Equal(60,r.SavingsRate); }
}
