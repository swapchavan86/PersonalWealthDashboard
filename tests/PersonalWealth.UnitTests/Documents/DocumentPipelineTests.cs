using PersonalWealth.Application.Documents;
using PersonalWealth.Application.Documents.Business;
using PersonalWealth.Application.Documents.Staging;
using PersonalWealth.Application.Documents.Tabular;
using PersonalWealth.Application.Documents.Validation;
using PersonalWealth.Infrastructure.Documents;
using PersonalWealth.Infrastructure.Documents.Business;
using PersonalWealth.Infrastructure.Documents.Staging;
using Xunit;

namespace PersonalWealth.UnitTests.Documents;

public sealed class DocumentPipelineTests
{
    [Fact]
    public void Template_selector_prefers_highest_version()
    {
        var selector = new DocumentTemplateSelector();
        var templates = new[]
        {
            new DocumentTemplate("bank", 1, new[] { ".csv" }),
            new DocumentTemplate("bank", 2, new[] { ".csv" })
        };

        Assert.Equal(2, selector.Select("csv", templates)!.Version);
    }

    [Fact]
    public void Structural_validator_reports_required_and_invalid_fields()
    {
        var template = new TabularTemplate("bank", 1, new[]
        {
            new TabularField("date", "Date", true, TabularFieldType.Date, null, null),
            new TabularField("amount", "Amount", true, TabularFieldType.Decimal, null, null)
        });
        var validator = new TabularDocumentValidator();

        var errors = validator.Validate("bank", 1, template, new[]
        {
            (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?> { ["Date"] = "bad", ["Amount"] = null }
        });

        Assert.Contains(errors, x => x.Code == "INVALID_DATE");
        Assert.Contains(errors, x => x.Code == "REQUIRED_FIELD_MISSING");
    }

    [Fact]
    public void Business_validator_rejects_zero_amount_and_missing_description()
    {
        var validator = new TransactionBusinessValidator();
        var result = validator.Validate(new[] { new NormalizedTransaction(DateTime.UtcNow, 0, "") });

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public async Task Staging_store_creates_auditable_record()
    {
        var store = new InMemoryImportStagingStore();
        var record = await store.CreateAsync(Guid.NewGuid(), 3);

        Assert.NotEqual(Guid.Empty, record.ImportId);
        Assert.Equal(ImportStatus.Staged, record.Status);
        Assert.Equal(3, record.StagedRowCount);
    }

    [Fact]
    public async Task Import_lifecycle_allows_validation_and_commit_but_rejects_invalid_transition()
    {
        var store = new InMemoryImportStagingStore();
        var lifecycle = new ImportLifecycleService(store);
        var record = await lifecycle.StageAsync(Guid.NewGuid(), 2);
        var validated = await lifecycle.TransitionAsync(record, ImportStatus.Validated, DateTimeOffset.UtcNow);
        var committed = await lifecycle.TransitionAsync(validated, ImportStatus.Committed, DateTimeOffset.UtcNow);

        Assert.Equal(ImportStatus.Committed, committed.Status);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            lifecycle.TransitionAsync(committed, ImportStatus.Failed, DateTimeOffset.UtcNow));
    }
}
