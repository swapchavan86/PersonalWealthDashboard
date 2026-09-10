using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using PersonalWealth.Application.Banking;
using PersonalWealth.Application.Documents.Tabular;

namespace PersonalWealth.Infrastructure.Banking;

public sealed class SampleBankCsvParser : IBankDocumentParser
{
    public BankParseResult Parse(string content, TabularTemplate template)
    {
        if (template.TemplateId != FirstBankStatementTemplate.TemplateId || template.Version != FirstBankStatementTemplate.Version)
            return new([], [new(0, "DOC_TEMPLATE_UNSUPPORTED", "Unsupported bank statement template.")], null, null);

        var lines = content.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) return new([], [new(0, "DOC_EMPTY", "Statement is empty.")], null, null);
        var header = lines[0].Split(',').Select(x => x.Trim()).ToArray();
        var required = new[] { "Date", "Description", "Debit", "Credit", "Balance", "AccountNumber" };
        if (!required.All(x => header.Contains(x, StringComparer.OrdinalIgnoreCase)))
            return new([], [new(1, "DOC_HEADER_INVALID", "Required statement columns are missing.")], null, null);

        var rows = new List<ParsedBankRow>();
        var errors = new List<BankParseError>();
        decimal? opening = null;
        decimal? closing = null;
        for (var i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(',');
            if (values.All(string.IsNullOrWhiteSpace) || values.All(x => x.Trim().Equals("Total", StringComparison.OrdinalIgnoreCase))) continue;
            if (values.Length != header.Length) { errors.Add(new(i + 1, "DOC_COLUMN_COUNT", "Column count does not match header.")); continue; }
            var map = header.Select((name, index) => (name, index)).ToDictionary(x => x.name, x => values[x.index], StringComparer.OrdinalIgnoreCase);
            if (!DateTime.TryParseExact(map["Date"].Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) { errors.Add(new(i + 1, "DOC_DATE_INVALID", "Transaction date is invalid.")); continue; }
            var debit = ParseDecimal(map["Debit"]);
            var credit = ParseDecimal(map["Credit"]);
            var balance = ParseDecimal(map["Balance"]);
            if (debit is null && credit is null) { errors.Add(new(i + 1, "DOC_AMOUNT_MISSING", "Debit or credit amount is required.")); continue; }
            if (debit is not null && credit is not null && debit != 0m && credit != 0m) { errors.Add(new(i + 1, "DOC_AMOUNT_AMBIGUOUS", "Debit and credit cannot both contain non-zero values.")); continue; }
            var amountValue = credit is not null && credit != 0m ? credit : debit;
            var amount = Math.Abs(amountValue ?? 0m);
            if (amount == 0m) { errors.Add(new(i + 1, "DOC_AMOUNT_ZERO", "Transaction amount must be greater than zero.")); continue; }
            var direction = credit is not null && credit != 0m ? "Credit" : "Debit";
            var description = string.Join(' ', map["Description"].Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            var account = map["AccountNumber"].Trim();
            if (string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(account)) { errors.Add(new(i + 1, "DOC_REQUIRED_VALUE", "Description and account number are required.")); continue; }
            opening ??= balance is not null ? balance - (direction == "Credit" ? amount : -amount) : null;
            closing = balance ?? closing;
            var canonical = $"{date:yyyy-MM-dd}|{amount:F2}|{direction}|{description}|{account}";
            var fingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
            rows.Add(new(i + 1, date, amount, direction, description, balance, account, fingerprint));
        }
        return new(rows, errors, opening, closing);
    }

    private static decimal? ParseDecimal(string value) =>
        decimal.TryParse(value.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var result) ? result : null;
}
