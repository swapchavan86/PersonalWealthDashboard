using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonalWealth.Application.Imports;
using PersonalWealth.Application.Tenancy;
using PersonalWealth.Domain.Assets;
using PersonalWealth.Domain.Banking;
using PersonalWealth.Domain.Expenses;
using PersonalWealth.Domain.Investments;
using PersonalWealth.Domain.Liabilities;
using PersonalWealth.Infrastructure.Persistence;

namespace PersonalWealth.Infrastructure.Documents;

public sealed class WealthWorkbookImportService(PersonalWealthDbContext db, ITenantContext tenantContext) : IWealthWorkbookImportService
{
    private static readonly string[] Headers =
    ["RecordType","ExternalId","Date","Institution","AccountNumber","AccountType","Currency","Description","Direction","Amount","Category","SecuritySymbol","SecurityName","SecurityType","Quantity","UnitPrice","Fees","AssetName","AssetType","AcquisitionValue","LiabilityName","LiabilityType","Principal","InterestRate","MaturityDate","PrincipalAmount","InterestAmount","ValuationValue"];

    public async Task<WealthImportResult> ImportAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        if (!fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return new(Guid.Empty, 0, 0, ["The current template format is CSV and is intentionally Excel-compatible. Please upload the downloaded PersonalWealth_Import_Template.csv file."], false);

        var importId = Guid.NewGuid();
        var errors = new List<string>();
        var rows = await ReadRowsAsync(content, cancellationToken);
        if (rows.Count == 0) return new(importId, 0, 0, ["The Data sheet contains no data rows."], false);
        if (!Headers.SequenceEqual(rows[0].Keys, StringComparer.OrdinalIgnoreCase))
            return new(importId, rows.Count, 0, ["Template headers do not match the supported Personal Wealth template."], false);

        var parsed = new List<ImportRow>();
        for (var i = 0; i < rows.Count; i++)
        {
            try { parsed.Add(Parse(rows[i], i + 2)); }
            catch (Exception ex) { errors.Add($"Row {i + 2}: {ex.Message}"); }
        }
        if (errors.Count > 0) return new(importId, rows.Count, 0, errors, false);

        var tenant = tenantContext.TenantId;
        var imported = 0;
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var bankAccounts = await db.BankAccounts.ToDictionaryAsync(x => x.AccountNumber, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var investmentAccounts = await db.InvestmentAccounts.ToDictionaryAsync(x => x.AccountNumber, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var securities = await db.Securities.ToDictionaryAsync(x => x.Symbol, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var assets = await db.Assets.ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var liabilities = await db.Liabilities.ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);
            var categories = await db.ExpenseCategories.ToDictionaryAsync(x => x.Name, StringComparer.OrdinalIgnoreCase, cancellationToken);

            foreach (var row in parsed.Where(x => x.RecordType == "BANK_ACCOUNT"))
            {
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.BankAccounts.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    var account = new BankAccount(id, tenant, Required(row.Institution), Required(row.AccountNumber), ParseEnum<BankAccountType>(row.AccountType), Required(row.Currency));
                    db.BankAccounts.Add(account); bankAccounts[account.AccountNumber] = account; imported++;
                }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "INVESTMENT_ACCOUNT"))
            {
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.InvestmentAccounts.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    var account = new InvestmentAccount(id, tenant, Required(row.Institution), Required(row.AccountNumber), ParseEnum<InvestmentAccountType>(row.AccountType), Required(row.Currency));
                    db.InvestmentAccounts.Add(account); investmentAccounts[account.AccountNumber] = account; imported++;
                }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "SECURITY"))
            {
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.Securities.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    var security = new Security(id, tenant, Required(row.SecuritySymbol), Required(row.SecurityName), ParseEnum<SecurityType>(row.SecurityType), Required(row.Currency));
                    db.Securities.Add(security); securities[security.Symbol] = security; imported++;
                }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "ASSET"))
            {
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.Assets.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    var asset = new Asset(id, tenant, Required(row.AssetName), ParseEnum<AssetType>(row.AssetType), Required(row.Currency), RequiredDecimal(row.AcquisitionValue, row.RowNumber, "AcquisitionValue"), RequiredDate(row.Date, row.RowNumber, "Date"));
                    db.Assets.Add(asset); assets[asset.Name] = asset; imported++;
                }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "LIABILITY"))
            {
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.Liabilities.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    var liability = new Liability(id, tenant, Required(row.LiabilityName), ParseEnum<LiabilityType>(row.LiabilityType), Required(row.Currency), RequiredDecimal(row.Principal, row.RowNumber, "Principal"), RequiredDecimal(row.InterestRate, row.RowNumber, "InterestRate"), RequiredDate(row.Date, row.RowNumber, "Date"), OptionalDate(row.MaturityDate));
                    db.Liabilities.Add(liability); liabilities[liability.Name] = liability; imported++;
                }
            }

            await db.SaveChangesAsync(cancellationToken);

            foreach (var row in parsed.Where(x => x.RecordType == "BANK_TRANSACTION"))
            {
                var account = Find(bankAccounts, row.AccountNumber, row.RowNumber, "bank account");
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.BankTransactions.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    var transaction = new BankTransaction(id, tenant, account.Id, RequiredDate(row.Date, row.RowNumber, "Date"), RequiredDecimal(row.Amount, row.RowNumber, "Amount"), ParseEnum<BankTransactionDirection>(row.Direction), Required(row.Description), importId, Fingerprint(row));
                    transaction.SetCategory(row.Category); db.BankTransactions.Add(transaction); imported++;
                }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "EXPENSE"))
            {
                var categoryName = Required(row.Category);
                if (!categories.TryGetValue(categoryName, out var category))
                {
                    category = new ExpenseCategory(StableId("EXPENSE_CATEGORY", categoryName), tenant, categoryName);
                    db.ExpenseCategories.Add(category); categories[categoryName] = category;
                }
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.Expenses.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    db.Expenses.Add(new Expense(id, tenant, category.Id, RequiredDate(row.Date, row.RowNumber, "Date"), RequiredDecimal(row.Amount, row.RowNumber, "Amount"), Required(row.Description))); imported++;
                }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "INVESTMENT_TRANSACTION"))
            {
                var account = Find(investmentAccounts, row.AccountNumber, row.RowNumber, "investment account");
                var security = Find(securities, row.SecuritySymbol, row.RowNumber, "security");
                var type = ParseEnum<InvestmentTransactionType>(row.Direction ?? row.Description);
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.InvestmentTransactions.AnyAsync(x => x.Id == id, cancellationToken))
                {
                    db.InvestmentTransactions.Add(new InvestmentTransaction(id, tenant, account.Id, security.Id, RequiredDate(row.Date, row.RowNumber, "Date"), type, RequiredDecimal(row.Quantity, row.RowNumber, "Quantity"), RequiredDecimal(row.UnitPrice, row.RowNumber, "UnitPrice"), row.Fees ?? 0m, Required(row.Currency), row.Description));
                    imported++;
                }
                var holdingId = StableId("HOLDING", $"{account.Id}:{security.Id}");
                var holding = await db.InvestmentHoldings.SingleOrDefaultAsync(x => x.Id == holdingId, cancellationToken);
                if (holding is null) { holding = new InvestmentHolding(holdingId, tenant, account.Id, security.Id); db.InvestmentHoldings.Add(holding); }
                switch (type)
                {
                    case InvestmentTransactionType.Buy: holding.ApplyBuy(RequiredDecimal(row.Quantity, row.RowNumber, "Quantity"), RequiredDecimal(row.UnitPrice, row.RowNumber, "UnitPrice"), row.Fees ?? 0m); break;
                    case InvestmentTransactionType.Sell: holding.ApplySell(RequiredDecimal(row.Quantity, row.RowNumber, "Quantity"), RequiredDecimal(row.UnitPrice, row.RowNumber, "UnitPrice"), row.Fees ?? 0m); break;
                }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "ASSET_VALUATION"))
            {
                var asset = Find(assets, row.AssetName, row.RowNumber, "asset");
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.AssetValuations.AnyAsync(x => x.Id == id, cancellationToken))
                { db.AssetValuations.Add(new AssetValuation(id, tenant, asset.Id, RequiredDate(row.Date, row.RowNumber, "Date"), RequiredDecimal(row.ValuationValue, row.RowNumber, "ValuationValue"), "User import")); imported++; }
            }
            foreach (var row in parsed.Where(x => x.RecordType == "LIABILITY_REPAYMENT"))
            {
                var liability = Find(liabilities, row.LiabilityName, row.RowNumber, "liability");
                var id = StableId(row.RecordType, row.ExternalId);
                if (!await db.LiabilityRepayments.AnyAsync(x => x.Id == id, cancellationToken))
                { var principal = row.PrincipalAmount ?? 0m; db.LiabilityRepayments.Add(new LiabilityRepayment(id, tenant, liability.Id, RequiredDate(row.Date, row.RowNumber, "Date"), RequiredDecimal(row.Amount, row.RowNumber, "Amount"), principal, row.InterestAmount ?? 0m)); liability.ApplyRepayment(principal); imported++; }
            }
            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return new(importId, rows.Count, imported, [], true);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            return new(importId, rows.Count, 0, [ex.Message], false);
        }
    }

    private sealed record ImportRow(int RowNumber, string RecordType, string ExternalId, string? Date, string? Institution, string? AccountNumber, string? AccountType, string? Currency, string? Description, string? Direction, decimal? Amount, string? Category, string? SecuritySymbol, string? SecurityName, string? SecurityType, decimal? Quantity, decimal? UnitPrice, decimal? Fees, string? AssetName, string? AssetType, decimal? AcquisitionValue, string? LiabilityName, string? LiabilityType, decimal? Principal, decimal? InterestRate, string? MaturityDate, decimal? PrincipalAmount, decimal? InterestAmount, decimal? ValuationValue);

    private static ImportRow Parse(Dictionary<string,string> row, int rowNumber) => new(rowNumber, Required(row["RecordType"]), Required(row["ExternalId"]), Null(row["Date"]), Null(row["Institution"]), Null(row["AccountNumber"]), Null(row["AccountType"]), Null(row["Currency"]), Null(row["Description"]), Null(row["Direction"]), Decimal(row["Amount"]), Null(row["Category"]), Null(row["SecuritySymbol"]), Null(row["SecurityName"]), Null(row["SecurityType"]), Decimal(row["Quantity"]), Decimal(row["UnitPrice"]), Decimal(row["Fees"]), Null(row["AssetName"]), Null(row["AssetType"]), Decimal(row["AcquisitionValue"]), Null(row["LiabilityName"]), Null(row["LiabilityType"]), Decimal(row["Principal"]), Decimal(row["InterestRate"]), Null(row["MaturityDate"]), Decimal(row["PrincipalAmount"]), Decimal(row["InterestAmount"]), Decimal(row["ValuationValue"]));

    private static async Task<List<Dictionary<string,string>>> ReadRowsAsync(Stream stream, CancellationToken ct)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, true, 4096, leaveOpen: true);
        var lines = new List<string>(); while (!reader.EndOfStream) { ct.ThrowIfCancellationRequested(); lines.Add(await reader.ReadLineAsync(ct) ?? ""); }
        if (lines.Count == 0) return [];
        var header = ParseCsvLine(lines[0]);
        var result = new List<Dictionary<string,string>>();
        foreach (var line in lines.Skip(1).Where(x => !string.IsNullOrWhiteSpace(x))) { var cells=ParseCsvLine(line); if(cells.Count!=header.Count) throw new InvalidOperationException("Every data row must contain the same number of columns as the header."); result.Add(header.Zip(cells,(h,v)=>(h,v)).ToDictionary(x=>x.h,x=>x.v,StringComparer.OrdinalIgnoreCase)); }
        return result;
    }
    private static List<string> ParseCsvLine(string line){var result=new List<string>();var sb=new StringBuilder();var quoted=false;for(var i=0;i<line.Length;i++){var c=line[i];if(c=='"'){if(quoted&&i+1<line.Length&&line[i+1]=='"'){sb.Append('"');i++;}else quoted=!quoted;}else if(c==','&&!quoted){result.Add(sb.ToString());sb.Clear();}else sb.Append(c);}if(quoted)throw new InvalidOperationException("CSV contains an unterminated quoted field.");result.Add(sb.ToString());return result;}
    private static string Required(string? value)=>string.IsNullOrWhiteSpace(value)?throw new InvalidOperationException("A required value is missing."):value.Trim();
    private static string? Null(string value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
    private static decimal? Decimal(string value)=>string.IsNullOrWhiteSpace(value)?null:decimal.Parse(value,CultureInfo.InvariantCulture);
    private static decimal RequiredDecimal(decimal? value,int row,string field)=>value??throw new InvalidOperationException($"Row {row}: {field} is required.");
    private static DateTime RequiredDate(string? value,int row,string field)=>DateTime.TryParse(value,CultureInfo.InvariantCulture,DateTimeStyles.None,out var d)?d:throw new InvalidOperationException($"Row {row}: {field} is required and must be a valid date.");
    private static DateTime? OptionalDate(string? value)=>DateTime.TryParse(value,CultureInfo.InvariantCulture,DateTimeStyles.None,out var d)?d:null;
    private static T ParseEnum<T>(string? value) where T:struct,Enum=>Enum.TryParse<T>(Required(value),true,out var result)?result:throw new InvalidOperationException($"Unsupported {typeof(T).Name} value '{value}'.");
    private static T Find<T>(Dictionary<string,T> map,string? key,int row,string type)=>map.TryGetValue(Required(key),out var value)?value:throw new InvalidOperationException($"Row {row}: referenced {type} '{key}' was not found.");
    private static Guid StableId(string type,string value){using var sha=SHA256.Create();var bytes=sha.ComputeHash(Encoding.UTF8.GetBytes($"PersonalWealth:{type}:{value}"));return new Guid(bytes.Take(16).ToArray());}
    private static string Fingerprint(ImportRow row)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("|",row.RecordType,row.ExternalId,row.Date,row.AccountNumber,row.Amount,row.Description,row.Direction))));
}
