using Microsoft.Extensions.Options;
using PersonalWealth.Infrastructure.Documents;
using Xunit;

namespace PersonalWealth.UnitTests.Documents;

public sealed class DocumentStorageTests
{
    [Fact]
    public void Scanner_returns_supported_files_in_deterministic_order_and_ignores_temporary_files()
    {
        var root = Path.Combine(Path.GetTempPath(), $"PersonalWealthTests_{Guid.NewGuid():N}");
        var incoming = Path.Combine(root, "incoming");
        Directory.CreateDirectory(incoming);

        try
        {
            File.WriteAllText(Path.Combine(incoming, "b.csv"), "data");
            File.WriteAllText(Path.Combine(incoming, "a.xlsx"), "data");
            File.WriteAllText(Path.Combine(incoming, "~$temp.xlsx"), "data");
            File.WriteAllText(Path.Combine(incoming, "ignore.txt"), "data");

            var options = Options.Create(new DocumentStorageOptions { RootPath = root });
            var scanner = new FileSystemDocumentScanner(options);
            var results = scanner.Discover();

            Assert.Equal(["a.xlsx", "b.csv"], results.Select(x => x.FileName));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public async Task Hasher_returns_same_sha256_for_same_content()
    {
        var root = Path.Combine(Path.GetTempPath(), $"PersonalWealthTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "statement.csv");

        try
        {
            await File.WriteAllTextAsync(path, "same-content");
            var hasher = new DocumentHasher();

            var first = await hasher.ComputeSha256Async(path);
            var second = await hasher.ComputeSha256Async(path);

            Assert.Equal(first, second);
            Assert.Equal(64, first.Length);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
