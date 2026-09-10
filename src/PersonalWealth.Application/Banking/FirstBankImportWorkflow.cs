using PersonalWealth.Application.Documents;
using PersonalWealth.Application.Documents.Business;

namespace PersonalWealth.Application.Banking;

public sealed record FirstBankWorkflowRequest(
    Guid TenantId,
    Guid AccountId,
    Guid ImportId,
    string ContentHash,
    string Extension,
    string Content,
    decimal? OpeningBalance = null,
    decimal? ClosingBalance = null);

public interface IFirstBankImportWorkflow
{
    Task<BankImportResult> ImportAsync(FirstBankWorkflowRequest request, CancellationToken cancellationToken = default);
}

public sealed class FirstBankImportWorkflow(
    IDocumentTemplateSelector templateSelector,
    IBankDocumentParser parser,
    ITransactionBusinessValidator businessValidator,
    IBankImportService importService) : IFirstBankImportWorkflow
{
    public Task<BankImportResult> ImportAsync(FirstBankWorkflowRequest request, CancellationToken cancellationToken = default)
    {
        var template = templateSelector.Select(request.Extension, [new DocumentTemplate(FirstBankStatementTemplate.TemplateId, FirstBankStatementTemplate.Version, [".csv"]) ]);
        if (template is null)
            return Task.FromResult(new BankImportResult(request.ImportId, 0, 0, false, ["No supported bank template was found."]));

        var parsed = parser.Parse(request.Content, FirstBankStatementTemplate.Definition);
        if (parsed.Errors.Count > 0)
            return Task.FromResult(new BankImportResult(request.ImportId, 0, 0, false, parsed.Errors.Select(x => $"Row {x.RowNumber}: {x.Message}").ToArray()));

        var transactions = parsed.Rows.Select(x => new NormalizedTransaction(x.TransactionDate, x.Amount, x.Description)).ToArray();
        var businessResult = businessValidator.Validate(transactions);
        if (!businessResult.IsValid)
            return Task.FromResult(new BankImportResult(request.ImportId, 0, 0, false, businessResult.Errors.Select(x => x.Message).ToArray()));

        var models = parsed.Rows.Select(row => new BankTransactionModel(Guid.NewGuid(), request.TenantId, request.AccountId, row.TransactionDate, row.Amount, row.Direction, row.Description, request.ImportId, row.Fingerprint, null, null)).ToArray();
        return importService.ImportAsync(new(request.TenantId, request.AccountId, request.ImportId, request.ContentHash, models, request.OpeningBalance ?? parsed.OpeningBalance, request.ClosingBalance ?? parsed.ClosingBalance), cancellationToken);
    }
}
