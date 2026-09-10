using Microsoft.Extensions.Options;

namespace PersonalWealth.Infrastructure.Documents;

public sealed class DocumentFolderProvider(IOptions<DocumentStorageOptions> options)
{
    private readonly DocumentStorageOptions options = options.Value;

    public string RootPath => Path.GetFullPath(options.RootPath);

    public string IncomingPath => GetPath(options.IncomingFolder);

    public string ProcessedPath => GetPath(options.ProcessedFolder);

    public string FailedPath => GetPath(options.FailedFolder);

    public string ArchivePath => GetPath(options.ArchiveFolder);

    public void EnsureFolders()
    {
        Directory.CreateDirectory(IncomingPath);
        Directory.CreateDirectory(ProcessedPath);
        Directory.CreateDirectory(FailedPath);
        Directory.CreateDirectory(ArchivePath);
    }

    private string GetPath(string folder) => Path.GetFullPath(Path.Combine(options.RootPath, folder));
}
