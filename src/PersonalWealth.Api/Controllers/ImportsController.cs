using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWealth.Application.Imports;
using PersonalWealth.Application.Investments;

namespace PersonalWealth.Api.Controllers;

[ApiController, Route("api/v1/imports"), Authorize]
public sealed class ImportsController(IInvestmentImportService investments, IWealthWorkbookImportService wealthImport) : ControllerBase
{
    [HttpPost("investments/validate")]
    public IActionResult ValidateInvestmentRows([FromBody] IReadOnlyList<InvestmentImportRow> rows) => Ok(investments.Validate(rows));

    [HttpPost("wealth")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> ImportWealth([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0) return BadRequest(new { error = "A non-empty CSV template file is required." });
        await using var stream = file.OpenReadStream();
        var result = await wealthImport.ImportAsync(stream, file.FileName, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
