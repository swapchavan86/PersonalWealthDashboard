using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalWealth.Application.Banking;
using PersonalWealth.Application.Documents;
using PersonalWealth.Application.Documents.Business;
using PersonalWealth.Application.Documents.Staging;
using PersonalWealth.Application.Expenses;
using PersonalWealth.Application.Investments;
using PersonalWealth.Infrastructure.Banking;
using PersonalWealth.Infrastructure.Documents.Business;
using PersonalWealth.Infrastructure.Documents.Staging;
using PersonalWealth.Infrastructure.Expenses;

namespace PersonalWealth.Infrastructure.Documents;

public static class DocumentServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DocumentStorageOptions>()
            .Bind(configuration.GetSection(DocumentStorageOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.RootPath));

        services.AddSingleton<DocumentFolderProvider>();
        services.AddSingleton<IDocumentScanner, FileSystemDocumentScanner>();
        services.AddSingleton<IDocumentHasher, DocumentHasher>();
        services.AddSingleton<IDocumentTemplateSelector, DocumentTemplateSelector>();
        services.AddSingleton<ITransactionBusinessValidator, TransactionBusinessValidator>();
        services.AddSingleton<IImportStagingStore, InMemoryImportStagingStore>();
        services.AddSingleton<ImportLifecycleService>();
        services.AddSingleton<IBankDocumentParser, SampleBankCsvParser>();
        services.AddScoped<IBankAccountRepository, EfBankAccountRepository>();
        services.AddScoped<IBankTransactionRepository, EfBankTransactionRepository>();
        services.AddScoped<ITransactionNormalizer, TransactionNormalizer>();
        services.AddScoped<ITransactionCategorizer, RuleBasedTransactionCategorizer>();
        services.AddScoped<ITransferDetector, TransferDetector>();
        services.AddScoped<IStatementReconciler, StatementReconciler>();
        services.AddScoped<IBankImportService, EfBankImportService>();
        services.AddScoped<IFirstBankImportWorkflow, FirstBankImportWorkflow>();
        services.AddScoped<IExpenseCategoryRepository, EfExpenseCategoryRepository>();
        services.AddScoped<IExpenseRepository, EfExpenseRepository>();
        services.AddScoped<IRecurringExpenseRepository, EfRecurringExpenseRepository>();
        services.AddScoped<IExpenseReportingRepository, EfExpenseReportingRepository>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IInvestmentPortfolioEngine, InvestmentPortfolioEngine>();
        services.AddScoped<IInvestmentValuationService, InvestmentValuationService>();

        return services;
    }
}
