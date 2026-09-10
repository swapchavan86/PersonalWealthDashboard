namespace PersonalWealth.Infrastructure.Documents;

public sealed class DocumentStorageOptions
{
    public const string SectionName = "DocumentStorage";

    public string RootPath { get; set; } = "data/documents";

    public string IncomingFolder { get; set; } = "incoming";

    public string ProcessedFolder { get; set; } = "processed";

    public string FailedFolder { get; set; } = "failed";

    public string ArchiveFolder { get; set; } = "archive";

    public string GetPath(string folder) => Path.Combine(RootPath, folder);
}
