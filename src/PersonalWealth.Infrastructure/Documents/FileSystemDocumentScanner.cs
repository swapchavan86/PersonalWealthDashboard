using Microsoft.Extensions.Options;
using PersonalWealth.Application.Documents;

namespace PersonalWealth.Infrastructure.Documents;

public sealed class FileSystemDocumentScanner(
    IOptions<DocumentStorageOptions> options)
    : IDocumentScanner
{
    private static readonly HashSet<string> SupportedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".csv",
            ".xlsx",
            ".xls",
            ".pdf"
        };

    private static readonly HashSet<string> IgnoredFileNames =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".gitkeep",
            "desktop.ini"
        };

    private readonly DocumentStorageOptions options = options.Value;

    public IReadOnlyList<DocumentDescriptor> Discover(CancellationToken cancellationToken = default)
    {
        var incomingPath = Path.GetFullPath(options.GetPath(options.IncomingFolder));
        if (!Directory.Exists(incomingPath))
        {
            return [];
        }

        var files = Directory.EnumerateFiles(incomingPath, "*", SearchOption.TopDirectoryOnly)
            .Where(path => SupportedExtensions.Contains(Path.GetExtension(path)))
            .Where(path => !IgnoredFileNames.Contains(Path.GetFileName(path)))
            .Where(path => !Path.GetFileName(path).StartsWith("~$", StringComparison.Ordinal))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var results = new List<DocumentDescriptor>(files.Length);
        foreach (var path in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var info = new FileInfo(path);
            results.Add(new DocumentDescriptor(
                info.FullName,
                info.Name,
                info.Extension,
                info.Length,
                info.LastWriteTimeUtc));
        }

        return results;
    }
}
