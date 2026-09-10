using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalWealth.Application.Documents;
using PersonalWealth.Application.Documents.Business;
using PersonalWealth.Application.Documents.Staging;
using PersonalWealth.Infrastructure.Documents.Business;
using PersonalWealth.Infrastructure.Documents.Staging;

namespace PersonalWealth.Infrastructure.Documents;

public static class DocumentServiceCollectionExtensions
{
    public static IServiceCollection AddDocumentServices(
        this IServiceCollection services,
        IConfiguration configuration)
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

        return services;
    }
}
