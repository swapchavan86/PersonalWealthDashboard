namespace PersonalWealth.Application.Documents.Staging;

public enum ImportStatus
{
    Discovered,
    Staged,
    Validated,
    Rejected,
    Committed,
    Failed
}
