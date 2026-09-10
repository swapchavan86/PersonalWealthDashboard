namespace PersonalWealth.Application.Assets;
public sealed record AssetImportRow(int RowNumber,string Name,string Type,string Currency,decimal AcquisitionValue,DateTime AcquisitionDate);
public sealed record AssetImportResult(IReadOnlyList<AssetImportRow> Accepted,IReadOnlyList<(int RowNumber,string Error)> Rejected);
public interface IAssetImportService{AssetImportResult Validate(IEnumerable<AssetImportRow> rows);}
public sealed class AssetImportService:IAssetImportService
{
 public AssetImportResult Validate(IEnumerable<AssetImportRow> rows){var ok=new List<AssetImportRow>();var bad=new List<(int,string)>();foreach(var r in rows.OrderBy(x=>x.RowNumber)){var e=new List<string>();if(string.IsNullOrWhiteSpace(r.Name))e.Add("Name is required.");if(string.IsNullOrWhiteSpace(r.Type))e.Add("Type is required.");if(string.IsNullOrWhiteSpace(r.Currency))e.Add("Currency is required.");if(r.AcquisitionValue<0)e.Add("Acquisition value cannot be negative.");if(r.AcquisitionDate==default)e.Add("Acquisition date is required.");if(e.Count==0)ok.Add(r);else bad.Add((r.RowNumber,string.Join(" ",e)));}return new(ok,bad);}
}
