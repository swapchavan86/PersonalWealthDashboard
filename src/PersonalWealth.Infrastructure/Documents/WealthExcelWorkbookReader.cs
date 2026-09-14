using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PersonalWealth.Infrastructure.Documents;

internal sealed record ExcelWorkbookReadResult(List<Dictionary<string, string>> Rows, List<string> Errors);

internal static class WealthExcelWorkbookReader
{
    private sealed record SheetDefinition(string Name, string[] Columns, string[] RequiredColumns, string[] DecimalColumns, string[] DateColumns, Dictionary<string, string[]> ValidValues);
    private static readonly SheetDefinition[] Definitions =
    [
        new("Bank Accounts", ["ExternalId","Institution","AccountNumber","AccountType","Currency"], ["ExternalId","Institution","AccountNumber","AccountType","Currency"], [], [], new() { ["AccountType"] = ["Savings","Current"] }),
        new("Bank Transactions", ["ExternalId","Date","AccountNumber","Currency","Description","Direction","Amount","Category"], ["ExternalId","Date","AccountNumber","Currency","Description","Direction","Amount"], ["Amount"], ["Date"], new() { ["Direction"] = ["Credit","Debit"] }),
        new("Expenses", ["ExternalId","Date","AccountNumber","Currency","Description","Amount","Category"], ["ExternalId","Date","AccountNumber","Currency","Description","Amount","Category"], ["Amount"], ["Date"], new()),
        new("Investment Accounts", ["ExternalId","Institution","AccountNumber","AccountType","Currency"], ["ExternalId","Institution","AccountNumber","AccountType","Currency"], [], [], new() { ["AccountType"] = ["Brokerage","MutualFund","Retirement","Other"] }),
        new("Securities", ["ExternalId","SecuritySymbol","SecurityName","SecurityType","Currency"], ["ExternalId","SecuritySymbol","SecurityName","SecurityType","Currency"], [], [], new() { ["SecurityType"] = ["Equity","MutualFund","Bond","Etf","Other"] }),
        new("Investment Transactions", ["ExternalId","Date","AccountNumber","Currency","Description","Direction","SecuritySymbol","Quantity","UnitPrice","Fees"], ["ExternalId","Date","AccountNumber","Currency","Direction","SecuritySymbol","Quantity","UnitPrice"], ["Quantity","UnitPrice","Fees"], ["Date"], new() { ["Direction"] = ["Buy","Sell"] }),
        new("Assets", ["ExternalId","Date","AssetName","AssetType","Currency","AcquisitionValue"], ["ExternalId","Date","AssetName","AssetType","Currency","AcquisitionValue"], ["AcquisitionValue"], ["Date"], new() { ["AssetType"] = ["Property","Vehicle","Cash","PreciousMetal","Business","Other"] }),
        new("Asset Valuations", ["ExternalId","Date","AssetName","ValuationValue"], ["ExternalId","Date","AssetName","ValuationValue"], ["ValuationValue"], ["Date"], new()),
        new("Liabilities", ["ExternalId","Date","LiabilityName","LiabilityType","Currency","Principal","InterestRate","MaturityDate"], ["ExternalId","Date","LiabilityName","LiabilityType","Currency","Principal","InterestRate"], ["Principal","InterestRate"], ["Date","MaturityDate"], new() { ["LiabilityType"] = ["Mortgage","PersonalLoan","CreditCard","EducationLoan","EPF","NPS","Other"] }),
        new("Liability Repayments", ["ExternalId","Date","LiabilityName","Currency","Amount","PrincipalAmount","InterestAmount"], ["ExternalId","Date","LiabilityName","Currency","Amount"], ["Amount","PrincipalAmount","InterestAmount"], ["Date"], new())
    ];
    private static readonly Dictionary<string, SheetDefinition> ByName = Definitions.ToDictionary(x => Normalize(x.Name), StringComparer.OrdinalIgnoreCase);

    public static ExcelWorkbookReadResult Read(Stream stream, CancellationToken cancellationToken)
    {
        var rows = new List<Dictionary<string, string>>(); var errors = new List<string>();
        using var document = SpreadsheetDocument.Open(stream, false);
        var workbookPart = document.WorkbookPart ?? throw new InvalidOperationException("The Excel workbook has no workbook part.");
        var sharedStrings = workbookPart.SharedStringTablePart?.SharedStringTable;
        foreach (var sheet in workbookPart.Workbook.Sheets?.Elements<Sheet>() ?? [])
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sheetName = sheet.Name?.Value ?? "Unnamed sheet";
            if (Normalize(sheetName) is "instructions" or "validation report") continue;
            if (!ByName.TryGetValue(Normalize(sheetName), out var definition)) { errors.Add($"Sheet '{sheetName}': unsupported sheet name. Use one of: {string.Join(", ", Definitions.Select(x => x.Name))}."); continue; }
            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
            var rowsInSheet = worksheetPart.Worksheet.GetFirstChild<SheetData>()?.Elements<Row>().ToList() ?? [];
            if (rowsInSheet.Count == 0) continue;
            var headerCells = rowsInSheet[0].Elements<Cell>().ToList();
            var headers = headerCells.Select((cell, index) => (Header: CellText(cell, sharedStrings), Column: ColumnLetter(cell.CellReference?.Value) ?? ExcelColumn(index + 1))).ToList();
            var expected = definition.Columns.Select(Normalize).ToArray();
            if (headers.Count != expected.Length || !headers.Select(x => Normalize(x.Header)).SequenceEqual(expected, StringComparer.OrdinalIgnoreCase)) { errors.Add($"Sheet '{sheetName}': columns are incorrect. Expected: {string.Join(", ", definition.Columns)}."); continue; }
            foreach (var row in rowsInSheet.Skip(1))
            {
                cancellationToken.ThrowIfCancellationRequested(); var rowNumber = (int)(row.RowIndex?.Value ?? 0); if (rowNumber == 0) continue;
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var cell in row.Elements<Cell>()) { var column = ColumnLetter(cell.CellReference?.Value); if (column is null) continue; var header = headers.FirstOrDefault(x => x.Column.Equals(column, StringComparison.OrdinalIgnoreCase)).Header; if (!string.IsNullOrWhiteSpace(header)) values[header] = CellText(cell, sharedStrings); }
                if (values.Count == 0 || values.Values.All(string.IsNullOrWhiteSpace)) continue;
                var rowErrors = new List<string>();
                foreach (var required in definition.RequiredColumns) if (!values.TryGetValue(required, out var value) || string.IsNullOrWhiteSpace(value)) rowErrors.Add($"Sheet '{sheetName}', cell {CellAddress(headers, required, rowNumber)}: {required} is required.");
                foreach (var field in definition.DecimalColumns) if (values.TryGetValue(field, out var value) && !string.IsNullOrWhiteSpace(value) && !decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _)) rowErrors.Add($"Sheet '{sheetName}', cell {CellAddress(headers, field, rowNumber)}: {field} must be a number, but '{value}' was provided.");
                foreach (var field in definition.DateColumns) if (values.TryGetValue(field, out var value) && !string.IsNullOrWhiteSpace(value) && !DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) rowErrors.Add($"Sheet '{sheetName}', cell {CellAddress(headers, field, rowNumber)}: {field} must be a valid date, but '{value}' was provided.");
                foreach (var pair in definition.ValidValues) if (values.TryGetValue(pair.Key, out var value) && !string.IsNullOrWhiteSpace(value) && !pair.Value.Contains(value, StringComparer.OrdinalIgnoreCase)) rowErrors.Add($"Sheet '{sheetName}', cell {CellAddress(headers, pair.Key, rowNumber)}: unsupported {pair.Key} value '{value}'. Allowed values: {string.Join(", ", pair.Value)}.");
                if (rowErrors.Count > 0) errors.AddRange(rowErrors); else { var unified = ToUnifiedRow(definition, values); unified["__Sheet"] = sheetName; unified["__RowNumber"] = rowNumber.ToString(CultureInfo.InvariantCulture); rows.Add(unified); }
            }
        }
        return new(rows, errors);
    }

    private static Dictionary<string, string> ToUnifiedRow(SheetDefinition definition, Dictionary<string, string> values)
    {
        var recordType = definition.Name switch { "Bank Accounts" => "BANK_ACCOUNT", "Bank Transactions" => "BANK_TRANSACTION", "Expenses" => "EXPENSE", "Investment Accounts" => "INVESTMENT_ACCOUNT", "Securities" => "SECURITY", "Investment Transactions" => "INVESTMENT_TRANSACTION", "Assets" => "ASSET", "Asset Valuations" => "ASSET_VALUATION", "Liabilities" => "LIABILITY", "Liability Repayments" => "LIABILITY_REPAYMENT", _ => definition.Name.ToUpperInvariant().Replace(" ", "_") };
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["RecordType"] = recordType }; foreach (var pair in values) map[pair.Key] = pair.Value; return map;
    }
    private static string CellText(Cell cell, SharedStringTable? sharedStrings) { var value = cell.CellValue?.Text ?? cell.InnerText ?? string.Empty; if (cell.DataType?.Value == CellValues.SharedString && int.TryParse(value, out var index) && sharedStrings is not null) return sharedStrings.Elements<SharedStringItem>().ElementAtOrDefault(index)?.InnerText ?? string.Empty; if (cell.DataType?.Value == CellValues.Boolean) return value == "1" ? "TRUE" : "FALSE"; return value.Trim(); }
    private static string CellAddress(List<(string Header, string Column)> headers, string field, int rowNumber) => $"{headers.First(x => x.Header.Equals(field, StringComparison.OrdinalIgnoreCase)).Column}{rowNumber}";
    private static string Normalize(string value) => value.Trim().Replace("_", " ").Replace("-", " ").ToLowerInvariant();
    private static string? ColumnLetter(string? reference) { if (string.IsNullOrWhiteSpace(reference)) return null; var letters = new string(reference.TakeWhile(char.IsLetter).ToArray()); return letters.Length == 0 ? null : letters.ToUpperInvariant(); }
    private static string ExcelColumn(int number) { var result = string.Empty; while (number > 0) { number--; result = (char)('A' + number % 26) + result; number /= 26; } return result; }
}
