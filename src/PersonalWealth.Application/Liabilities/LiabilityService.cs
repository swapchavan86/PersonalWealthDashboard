using PersonalWealth.Domain.Liabilities;

namespace PersonalWealth.Application.Liabilities;

public sealed record LoanScheduleLine(int Period, DateTime DueDate, decimal Payment, decimal Interest, decimal Principal, decimal Balance);
public interface ILoanScheduleService { IReadOnlyList<LoanScheduleLine> Build(Liability liability, decimal monthlyPayment, int periods); }

public sealed class LoanScheduleService : ILoanScheduleService
{
    public IReadOnlyList<LoanScheduleLine> Build(Liability liability, decimal monthlyPayment, int periods)
    {
        if (monthlyPayment <= 0) throw new ArgumentOutOfRangeException(nameof(monthlyPayment)); if (periods <= 0) throw new ArgumentOutOfRangeException(nameof(periods));
        var balance = liability.OutstandingPrincipal; var rate = liability.AnnualInterestRate / 100m / 12m; var result = new List<LoanScheduleLine>();
        for (var i = 1; i <= periods && balance > 0; i++) { var interest = decimal.Round(balance * rate, 4, MidpointRounding.ToEven); var principal = Math.Min(balance, Math.Max(0, monthlyPayment - interest)); var payment = principal + interest; balance = decimal.Round(balance - principal, 4, MidpointRounding.ToEven); result.Add(new LoanScheduleLine(i, liability.StartDate.AddMonths(i), payment, interest, principal, balance)); }
        return result;
    }
}
