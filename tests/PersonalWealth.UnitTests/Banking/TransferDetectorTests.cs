using PersonalWealth.Application.Banking;
using Xunit;

namespace PersonalWealth.UnitTests.Banking;

public sealed class TransferDetectorTests
{
    [Fact]
    public void Detect_matches_same_tenant_different_accounts_same_amount_within_one_day()
    {
        var detector = new TransferDetector();
        var tenant = Guid.NewGuid();
        var debit = CreateTransaction(tenant, Guid.NewGuid(), new DateTime(2026, 1, 10), 1000m, "Debit", "d");
        var credit = CreateTransaction(tenant, Guid.NewGuid(), new DateTime(2026, 1, 11), 1000m, "Credit", "c");

        var matches = detector.Detect([debit, credit]);

        var match = Assert.Single(matches);
        Assert.Equal(debit.Id, match.DebitTransactionId);
        Assert.Equal(credit.Id, match.CreditTransactionId);
        Assert.Equal(1000m, match.Amount);
        Assert.Equal(TimeSpan.FromDays(1), match.DateDifference);
    }

    [Fact]
    public void Detect_does_not_cross_tenant_boundaries_or_match_same_account()
    {
        var detector = new TransferDetector();
        var tenant = Guid.NewGuid();
        var debitAccount = Guid.NewGuid();
        var debit = CreateTransaction(tenant, debitAccount, new DateTime(2026, 1, 10), 1000m, "Debit", "d");
        var wrongTenantCredit = CreateTransaction(Guid.NewGuid(), Guid.NewGuid(), debit.TransactionDate, 1000m, "Credit", "c1");
        var sameAccountCredit = CreateTransaction(tenant, debitAccount, debit.TransactionDate, 1000m, "Credit", "c2");

        var matches = detector.Detect([debit, wrongTenantCredit, sameAccountCredit]);

        Assert.Empty(matches);
    }

    [Fact]
    public void Detect_does_not_match_amounts_outside_one_day_window()
    {
        var detector = new TransferDetector();
        var tenant = Guid.NewGuid();
        var debit = CreateTransaction(tenant, Guid.NewGuid(), new DateTime(2026, 1, 10), 1000m, "Debit", "d");
        var credit = CreateTransaction(tenant, Guid.NewGuid(), new DateTime(2026, 1, 12), 1000m, "Credit", "c");

        var matches = detector.Detect([debit, credit]);

        Assert.Empty(matches);
    }

    [Fact]
    public void Detect_does_not_match_different_amounts()
    {
        var detector = new TransferDetector();
        var tenant = Guid.NewGuid();
        var debit = CreateTransaction(tenant, Guid.NewGuid(), new DateTime(2026, 1, 10), 1000m, "Debit", "d");
        var credit = CreateTransaction(tenant, Guid.NewGuid(), new DateTime(2026, 1, 10), 1000.01m, "Credit", "c");

        var matches = detector.Detect([debit, credit]);

        Assert.Empty(matches);
    }

    private static BankTransactionModel CreateTransaction(
        Guid tenantId,
        Guid accountId,
        DateTime date,
        decimal amount,
        string direction,
        string fingerprint) =>
        new(Guid.NewGuid(), tenantId, accountId, date, amount, direction, "Transfer", Guid.NewGuid(), fingerprint, null, null);
}
