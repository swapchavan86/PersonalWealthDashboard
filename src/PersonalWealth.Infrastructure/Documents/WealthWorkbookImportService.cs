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
        var lines = new List<string>();
        while (true)
        {
            ct.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(ct);
            if (line is null) break;
            lines.Add(line);
        }
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