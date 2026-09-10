namespace PersonalWealth.Application.Banking;

public sealed record TransferMatch(
    Guid DebitTransactionId,
    Guid CreditTransactionId,
    decimal Amount,
    TimeSpan DateDifference);

public interface ITransferDetector
{
    IReadOnlyCollection<TransferMatch> Detect(IReadOnlyCollection<BankTransactionModel> transactions);
}

public sealed class TransferDetector : ITransferDetector
{
    private static readonly TimeSpan MatchingWindow = TimeSpan.FromDays(1);

    public IReadOnlyCollection<TransferMatch> Detect(IReadOnlyCollection<BankTransactionModel> transactions)
    {
        var debits = transactions
            .Where(x => string.Equals(x.Direction, "Debit", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        var credits = transactions
            .Where(x => string.Equals(x.Direction, "Credit", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var matches = new List<TransferMatch>();
        var matchedCreditIds = new HashSet<Guid>();

        foreach (var debit in debits)
        {
            var candidate = credits
                .Where(credit =>
                    credit.TenantId == debit.TenantId &&
                    credit.AccountId != debit.AccountId &&
                    credit.Amount == debit.Amount &&
                    !matchedCreditIds.Contains(credit.Id))
                .Select(credit => new
                {
                    Credit = credit,
                    Difference = (credit.TransactionDate - debit.TransactionDate).Duration()
                })
                .Where(x => x.Difference <= MatchingWindow)
                .OrderBy(x => x.Difference)
                .FirstOrDefault();

            if (candidate is null)
                continue;

            matches.Add(new TransferMatch(debit.Id, candidate.Credit.Id, debit.Amount, candidate.Difference));
            matchedCreditIds.Add(candidate.Credit.Id);
        }

        return matches;
    }
}
