using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalWealth.Application.Investments;
namespace PersonalWealth.Api.Controllers;
[ApiController,Route("api/v1/imports"),Authorize]
public sealed class ImportsController(IInvestmentImportService investments) : ControllerBase
{
    [HttpPost("investments/validate")]
    public IActionResult ValidateInvestmentRows([FromBody] IReadOnlyList<InvestmentImportRow> rows) => Ok(investments.Validate(rows));
}
